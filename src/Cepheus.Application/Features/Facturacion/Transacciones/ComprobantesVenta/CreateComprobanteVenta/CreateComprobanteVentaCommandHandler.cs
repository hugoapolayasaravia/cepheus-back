using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.CreateComprobanteVenta;

public sealed class CreateComprobanteVentaCommandHandler
    : IRequestHandler<CreateComprobanteVentaCommand, ComprobanteVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateComprobanteVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ComprobanteVentaResponse> Handle(CreateComprobanteVentaCommand request, CancellationToken ct)
    {
        var type = ComprobanteVentaRules.NormalizeType(request.TypeCode);
        var numbers = request.Guias.Select(GuiaRemisionVentaRules.NormalizeNumber).Distinct().ToList();

        var guias = await _uow.Facturacion.Transacciones.GuiasRemisionVenta
            .Query()
            .Include(g => g.Detalles)
            .Where(g => numbers.Contains(g.Number))
            .ToListAsync(ct);

        var missing = numbers.Except(guias.Select(g => g.Number)).ToList();

        if (missing.Count > 0)
        {
            throw new KeyNotFoundException($"No existen las guías: {string.Join(", ", missing)}.");
        }

        var first = guias.OrderBy(g => g.IssueDate).ThenBy(g => g.Number).First();

        var date = (request.IssueDate ?? DateTime.Today).Date;
        ComprobanteVentaRules.EnsureIssueDate(date);
        await GuiaRemisionVentaRules.EnsurePeriodOpenAsync(_uow, date, ct);

        var cliente = await ComprobanteVentaRules.LoadClienteAsync(_uow, first.ClienteCode, ct);
        ComprobanteVentaRules.EnsureRuc(type, cliente);
        var obra = await GuiaRemisionVentaRules.LoadObraAsync(_uow, first.ClienteCode, first.ObraCode, ct);

        OrdenTrabajoVenta? ot = null;

        if (first.OrdenTrabajoCode is not null)
        {
            ot = await _uow.Facturacion.Transacciones.OrdenesTrabajoVenta
                .Query().AsNoTracking()
                .Include(o => o.Aprobacion)
                .FirstOrDefaultAsync(o => o.Code == first.OrdenTrabajoCode, ct)
                ?? throw new KeyNotFoundException($"La Orden de Trabajo {first.OrdenTrabajoCode} no existe.");

            if (ot.Status is not (EstadoOrdenTrabajoVenta.Activo or EstadoOrdenTrabajoVenta.Cerrado))
            {
                throw new InvalidOperationException($"La Orden de Trabajo {ot.Code} está {ot.Status}; no se factura.");
            }
        }

        var assigned = guias.Where(g => g.Document1Number is not null).Select(g => g.Document1Number!).Distinct().ToList();

        if (assigned.Count > 1)
        {
            throw new InvalidOperationException($"Las guías tienen documentos distintos asignados ({string.Join(", ", assigned)}).");
        }

        var plantCode = string.IsNullOrWhiteSpace(request.PlantCode)
            ? first.PlantCode
            : OrdenTrabajoVentaRules.NormalizeFixed(request.PlantCode, 2);

        var planta = await GuiaRemisionVentaRules.LoadPlantaAsync(_uow, plantCode, tracked: true, ct);

        var payment = obra.FormaPagoVentaCode ?? ot?.Aprobacion?.FormaPagoVentaCode
                      ?? throw new InvalidOperationException("La obra no tiene forma de pago de venta.");

        var currency = first.CurrencyCode;

        var c = new ComprobanteVenta
        {
            TypeCode = type,
            PlantCode = plantCode,
            ExecutingPlantCode = string.IsNullOrWhiteSpace(request.ExecutingPlantCode)
                ? plantCode
                : OrdenTrabajoVentaRules.NormalizeFixed(request.ExecutingPlantCode, 2),
            IssueDate = date,
            ClienteCode = first.ClienteCode,
            ObraCode = first.ObraCode,
            VendedorCode = ot?.VendedorCode ?? first.VendedorCode,
            Address = Cut(string.IsNullOrWhiteSpace(obra.DeliveryAddress) ? obra.Address : obra.DeliveryAddress, 100),
            UbigeoCode = string.IsNullOrWhiteSpace(obra.DeliveryUbigeoCode) ? obra.UbigeoCode : obra.DeliveryUbigeoCode,
            PaymentFormCode = payment,
            CurrencyCode = currency,
            ExchangeRate = await GuiaRemisionVentaRules.GetExchangeRateAsync(_uow, currency, date, ct),
            IgvPercentage = await ComprobanteVentaRules.GetIgvPercentageAsync(_uow, date, ct),
            OrdenTrabajoCode = first.OrdenTrabajoCode,
            Origin = OrigenComprobanteVenta.Guias,
            DocumentFlow = ot?.Aprobacion?.DocumentFlow ?? first.DocumentFlow,
            Glosa = OrdenTrabajoVentaRules.NormalizeText(request.Glosa),
            Status = EstadoComprobanteVenta.Pendiente
        };

        if (!await _uow.Comunes.Plantas.Query().AsNoTracking().AnyAsync(p => p.Code == c.ExecutingPlantCode, ct))
        {
            throw new KeyNotFoundException($"La planta ejecutora {c.ExecutingPlantCode} no existe.");
        }

        c.Number = await ComprobanteVentaRules.TakeNumberAsync(_uow, planta, type, assigned.FirstOrDefault(), request.Number, ct);

        ComprobanteVentaRules.AttachGuias(c, guias);
        ComprobanteVentaRules.Recalc(c);

        await _uow.Facturacion.Transacciones.ComprobantesVenta.AddAsync(c, ct);
        await ComprobanteVentaRules.SaveAsync(_uow, "el comprobante de pago", ct);

        return await ComprobanteVentaReader.GetAsync(_uow, c.TypeCode, c.Number, ct);
    }

    private static string Cut(string value, int max) => value.Length <= max ? value : value[..max];
}
