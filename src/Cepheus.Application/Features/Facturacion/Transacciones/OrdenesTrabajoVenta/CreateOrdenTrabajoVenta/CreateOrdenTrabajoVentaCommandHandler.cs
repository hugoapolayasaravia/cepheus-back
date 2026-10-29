using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.CreateOrdenTrabajoVenta;

public sealed class CreateOrdenTrabajoVentaCommandHandler
    : IRequestHandler<CreateOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateOrdenTrabajoVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(CreateOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        if (request.Type is TipoOrdenTrabajoVenta.Saldo or TipoOrdenTrabajoVenta.Consolidacion)
        {
            throw new InvalidOperationException(
                "Las OT de tipo Saldo y Consolidación no se crean a mano: las genera la fase de programación.");
        }

        var clienteCode = OrdenTrabajoVentaRules.NormalizeFixed(request.ClienteCode, 5);
        var obraCode = OrdenTrabajoVentaRules.NormalizeFixed(request.ObraCode, 3);

        var (_, obra) = await OrdenTrabajoVentaMaestros.GetActiveClienteObraAsync(_uow, clienteCode, obraCode, ct);

        var ot = new OrdenTrabajoVenta
        {
            ClienteCode = clienteCode,
            ObraCode = obraCode,
            Type = request.Type,
            Stage = EtapaOrdenTrabajoVenta.Pendiente,
            Status = EstadoOrdenTrabajoVenta.Activo,
            IsModifiable = true,
            PassedToProduction = false,
            CreditObservations = string.Empty,
            Total = 0m
        };

        // Origen: solo el tipo Descuento (OT de Anticipo que se descuenta).
        if (request.Type == TipoOrdenTrabajoVenta.Descuento)
        {
            if (string.IsNullOrWhiteSpace(request.OriginCode))
            {
                throw new InvalidOperationException("La OT Descuento requiere la OT de Anticipo de origen (OriginCode).");
            }

            var originCode = OrdenTrabajoVentaRules.NormalizeCode(request.OriginCode);

            var origin = await _uow.Facturacion.Transacciones.OrdenesTrabajoVenta
                .Query().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == originCode, ct)
                ?? throw new KeyNotFoundException($"La Orden de Trabajo de Anticipo {originCode} no existe.");

            if (origin.Type != TipoOrdenTrabajoVenta.Anticipo)
            {
                throw new InvalidOperationException($"La Orden de Trabajo {originCode} no es de tipo Anticipo.");
            }

            if (origin.Stage is EtapaOrdenTrabajoVenta.Pendiente or EtapaOrdenTrabajoVenta.Anulada
                || origin.Status != EstadoOrdenTrabajoVenta.Activo)
            {
                throw new InvalidOperationException(
                    $"La OT de Anticipo {originCode} debe estar Activa y aprobada (hoy: {origin.Stage}/{origin.Status}).");
            }

            if (origin.ClienteCode != clienteCode || origin.ObraCode != obraCode)
            {
                throw new InvalidOperationException("La OT de Anticipo pertenece a otro cliente u obra.");
            }

            var balance = await OrdenTrabajoVentaLineas.GetAnticipoBalanceAsync(_uow, originCode, null, ct);

            if (balance <= 0m)
            {
                throw new InvalidOperationException($"La OT de Anticipo {originCode} no tiene saldo disponible (0.00).");
            }

            ot.OriginCode = originCode;
        }
        else if (!string.IsNullOrWhiteSpace(request.OriginCode))
        {
            throw new InvalidOperationException("Solo la OT de tipo Descuento admite OT de origen.");
        }

        // Cotización (una sola por OT).
        CotizacionScope? cot = null;

        if (!string.IsNullOrWhiteSpace(request.CotizacionCode))
        {
            if (request.Type != TipoOrdenTrabajoVenta.Normal)
            {
                throw new InvalidOperationException("Solo la OT de tipo Normal se genera desde una Cotización.");
            }

            cot = await OrdenTrabajoVentaCotizacion.LoadAsync(
                _uow,
                OrdenTrabajoVentaRules.Normalize(request.CotizacionNegocioCode!),
                OrdenTrabajoVentaRules.Normalize(request.CotizacionYear!),
                OrdenTrabajoVentaRules.NormalizeFixed(request.CotizacionMonth!, 2),
                OrdenTrabajoVentaRules.Normalize(request.CotizacionCode!),
                ct);

            OrdenTrabajoVentaCotizacion.EnsureUsable(cot!.Cotizacion, clienteCode, obraCode);

            ot.CotizacionNegocioCode = cot.Cotizacion.NegocioCode;
            ot.CotizacionYear = cot.Cotizacion.Year;
            ot.CotizacionMonth = cot.Cotizacion.Month;
            ot.CotizacionCode = cot.Cotizacion.Code;
        }

        await OrdenTrabajoVentaCabecera.ApplyAsync(_uow, ot, request, obra, cot?.Cotizacion.CurrencyCode, ct);

        ot.Code = await OrdenTrabajoVentaRules.NextCodeAsync(_uow, ct);

        if (request.Detalles.Count > 0)
        {
            var ctx = new LineContext
            {
                Ot = ot,
                Cotizacion = cot,
                HasHojasCorte = false,
                OriginNegocios = ot.OriginCode is null
                    ? null
                    : (await _uow.Facturacion.Transacciones.OrdenesTrabajoVentaDetalle
                        .Query().AsNoTracking()
                        .Where(d => d.OrdenTrabajoCode == ot.OriginCode)
                        .Select(d => d.NegocioCode).Distinct().ToListAsync(ct))
                        .ToHashSet(StringComparer.Ordinal)
            };

            foreach (var d in request.Detalles)
            {
                await OrdenTrabajoVentaLineas.AddAsync(_uow, ctx, d, ct);
            }

            OrdenTrabajoVentaRules.Recalculate(ot);
            await OrdenTrabajoVentaLineas.EnsureAnticipoBalanceAsync(_uow, ot, ct);
            await OrdenTrabajoVentaLineas.EnsureCreditAsync(_uow, ot, ct);
        }

        await _uow.Facturacion.Transacciones.OrdenesTrabajoVenta.AddAsync(ot, ct);
        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, ot.Code, ct);
    }
}
