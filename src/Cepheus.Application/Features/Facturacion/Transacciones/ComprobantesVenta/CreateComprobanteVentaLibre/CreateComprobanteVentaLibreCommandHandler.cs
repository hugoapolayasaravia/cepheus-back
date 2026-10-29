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

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.CreateComprobanteVentaLibre;

public sealed class CreateComprobanteVentaLibreCommandHandler
    : IRequestHandler<CreateComprobanteVentaLibreCommand, ComprobanteVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public CreateComprobanteVentaLibreCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ComprobanteVentaResponse> Handle(CreateComprobanteVentaLibreCommand request, CancellationToken ct)
    {
        var type = ComprobanteVentaRules.NormalizeType(request.TypeCode);
        var date = (request.IssueDate ?? DateTime.Today).Date;
        ComprobanteVentaRules.EnsureIssueDate(date);
        await GuiaRemisionVentaRules.EnsurePeriodOpenAsync(_uow, date, ct);

        var clienteCode = OrdenTrabajoVentaRules.NormalizeFixed(request.ClienteCode, 5);
        var cliente = await ComprobanteVentaRules.LoadClienteAsync(_uow, clienteCode, ct);
        ComprobanteVentaRules.EnsureRuc(type, cliente);

        var obra = await GuiaRemisionVentaRules.LoadObraAsync(
            _uow, clienteCode, OrdenTrabajoVentaRules.NormalizeFixed(request.ObraCode, 3), ct);

        var currency = string.IsNullOrWhiteSpace(request.CurrencyCode)
            ? GuiaRemisionVentaRules.LocalCurrency
            : request.CurrencyCode.Trim().ToUpperInvariant();

        if (!await _uow.Comunes.Monedas.Query().AsNoTracking().AnyAsync(m => m.Code == currency && m.IsActive, ct))
        {
            throw new KeyNotFoundException($"La moneda {currency} no existe o está inactiva.");
        }

        var plantCode = OrdenTrabajoVentaRules.NormalizeFixed(request.PlantCode, 2);
        var planta = await GuiaRemisionVentaRules.LoadPlantaAsync(_uow, plantCode, tracked: true, ct);

        var payment = obra.FormaPagoVentaCode
                      ?? throw new InvalidOperationException("La obra no tiene forma de pago de venta.");

        var ubigeo = string.IsNullOrWhiteSpace(request.UbigeoCode)
            ? (string.IsNullOrWhiteSpace(obra.DeliveryUbigeoCode) ? obra.UbigeoCode : obra.DeliveryUbigeoCode)
            : OrdenTrabajoVentaRules.NormalizeFixed(request.UbigeoCode, 6);

        if (!await _uow.Comunes.Ubigeos.Query().AsNoTracking().AnyAsync(u => u.Code == ubigeo, ct))
        {
            throw new KeyNotFoundException($"El ubigeo {ubigeo} no existe.");
        }

        var address = OrdenTrabajoVentaRules.NormalizeText(request.Address)
                      ?? (string.IsNullOrWhiteSpace(obra.DeliveryAddress) ? obra.Address : obra.DeliveryAddress);

        var c = new ComprobanteVenta
        {
            TypeCode = type,
            PlantCode = plantCode,
            ExecutingPlantCode = string.IsNullOrWhiteSpace(request.ExecutingPlantCode)
                ? plantCode
                : OrdenTrabajoVentaRules.NormalizeFixed(request.ExecutingPlantCode, 2),
            IssueDate = date,
            ClienteCode = clienteCode,
            ObraCode = obra.Code,
            VendedorCode = string.IsNullOrWhiteSpace(request.VendedorCode)
                ? obra.VendedorCode
                : OrdenTrabajoVentaRules.NormalizeFixed(request.VendedorCode, 4),
            Address = address.Length <= 100 ? address : address[..100],
            UbigeoCode = ubigeo,
            PaymentFormCode = payment,
            CurrencyCode = currency,
            ExchangeRate = await GuiaRemisionVentaRules.GetExchangeRateAsync(_uow, currency, date, ct),
            IgvPercentage = await ComprobanteVentaRules.GetIgvPercentageAsync(_uow, date, ct),
            Origin = OrigenComprobanteVenta.Libre,
            Glosa = OrdenTrabajoVentaRules.NormalizeText(request.Glosa),
            Status = EstadoComprobanteVenta.Pendiente
        };

        if (!await _uow.Comunes.Plantas.Query().AsNoTracking().AnyAsync(p => p.Code == c.ExecutingPlantCode, ct))
        {
            throw new KeyNotFoundException($"La planta ejecutora {c.ExecutingPlantCode} no existe.");
        }

        c.Number = await ComprobanteVentaRules.TakeNumberAsync(_uow, planta, type, null, request.Number, ct);

        // PB ue_documento_libre: solo un número que Créditos no haya aprobado para una OT.
        if (await OrdenTrabajoVentaDocumentos.ExistsInOtherOrderAsync(
                _uow, ComprobanteVentaRules.ToApprovalType(type), c.Number, string.Empty, ct))
        {
            throw new InvalidOperationException(
                $"El número {c.Number} está aprobado por Créditos para una Orden de Trabajo; no se puede usar en un comprobante libre.");
        }

        foreach (var l in await ComprobanteVentaLibreLines.BuildAsync(_uow, c, request.Detalles, ct))
        {
            c.Detalles.Add(l);
        }

        ComprobanteVentaRules.Recalc(c);

        await _uow.Facturacion.Transacciones.ComprobantesVenta.AddAsync(c, ct);
        await ComprobanteVentaRules.SaveAsync(_uow, "el comprobante libre", ct);

        return await ComprobanteVentaReader.GetAsync(_uow, c.TypeCode, c.Number, ct);
    }
}
