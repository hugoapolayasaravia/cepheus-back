using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.CopiarOrdenTrabajoVenta;

public sealed class CopiarOrdenTrabajoVentaCommandHandler
    : IRequestHandler<CopiarOrdenTrabajoVentaCommand, OrdenTrabajoVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public CopiarOrdenTrabajoVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<OrdenTrabajoVentaResponse> Handle(CopiarOrdenTrabajoVentaCommand request, CancellationToken ct)
    {
        var sourceCode = OrdenTrabajoVentaRules.NormalizeCode(request.Code);

        var source = await _uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query().AsNoTracking()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(x => x.Code == sourceCode, ct)
            ?? throw new KeyNotFoundException($"La Orden de Trabajo {sourceCode} no existe.");

        if (source.Type is TipoOrdenTrabajoVenta.Saldo or TipoOrdenTrabajoVenta.Consolidacion)
        {
            throw new InvalidOperationException(
                "Las OT de tipo Saldo y Consolidación no se copian: las genera la fase de programación.");
        }

        if (source.Detalles.Any(d => d.NegocioCode == OrdenTrabajoVentaRules.NegocioConcreto))
        {
            throw new InvalidOperationException(
                "La OT tiene líneas de Concreto (CO), que exigen Cotización; la copia no lleva cotización y no es posible.");
        }

        await OrdenTrabajoVentaMaestros.GetActiveClienteObraAsync(_uow, source.ClienteCode, source.ObraCode, ct);

        var copy = new OrdenTrabajoVenta
        {
            Code = await OrdenTrabajoVentaRules.NextCodeAsync(_uow, ct),
            ClienteCode = source.ClienteCode,
            ObraCode = source.ObraCode,
            Type = source.Type,
            Stage = EtapaOrdenTrabajoVenta.Pendiente,
            Status = EstadoOrdenTrabajoVenta.Activo,
            IsModifiable = true,
            PassedToProduction = false,
            CreditObservations = string.Empty,
            DispatchDate = source.DispatchDate,
            DispatchTime = source.DispatchTime,
            VendedorCode = source.VendedorCode,
            MotivoCode = source.MotivoCode,
            CurrencyCode = source.CurrencyCode,
            Observations = source.Observations,

            // Descuento conserva el anticipo que descuenta; el resto guarda la OT copiada.
            OriginCode = source.Type == TipoOrdenTrabajoVenta.Descuento ? source.OriginCode : source.Code
        };

        foreach (var d in source.Detalles
                     .Where(x => x.Status != EstadoDetalleOrdenTrabajoVenta.Anulado)
                     .OrderBy(x => x.Item))
        {
            copy.Detalles.Add(new OrdenTrabajoVentaDetalle
            {
                OrdenTrabajoCode = copy.Code,
                ProductoTipoCode = d.ProductoTipoCode,
                ProductoCode = d.ProductoCode,
                NegocioCode = d.NegocioCode,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Total = d.Total,
                Status = EstadoDetalleOrdenTrabajoVenta.Pendiente
            });
        }

        OrdenTrabajoVentaRules.Recalculate(copy);
        await OrdenTrabajoVentaLineas.EnsureAnticipoBalanceAsync(_uow, copy, ct);

        await _uow.Facturacion.Transacciones.OrdenesTrabajoVenta.AddAsync(copy, ct);
        await OrdenTrabajoVentaReader.SaveAsync(_uow, ct);

        return await OrdenTrabajoVentaReader.GetAsync(_uow, copy.Code, ct);
    }
}
