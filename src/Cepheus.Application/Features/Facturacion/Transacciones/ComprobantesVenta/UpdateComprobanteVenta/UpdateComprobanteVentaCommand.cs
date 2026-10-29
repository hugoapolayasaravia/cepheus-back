using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.UpdateComprobanteVenta;

public sealed class UpdateComprobanteVentaRequest
{
    /// <summary>Nueva fecha de emisión (recalcula IGV y tipo de cambio; solo sin imprimir).</summary>
    public DateTime? IssueDate { get; set; }

    public string? Glosa { get; set; }
    public string? ExecutingPlantCode { get; set; }

    /// <summary>Detracción: documento del banco y monto (en soles).</summary>
    public string? DetractionDocument { get; set; }
    public decimal? DetractionAmount { get; set; }

    public string? CashDocument { get; set; }

    /// <summary>Solo comprobantes libres: conjunto final de líneas. Null = no cambia.</summary>
    public List<ComprobanteVentaLineaLibreRequest>? Detalles { get; set; }
}

public sealed record UpdateComprobanteVentaCommand(string TypeCode, string Number, UpdateComprobanteVentaRequest Data)
    : IRequest<ComprobanteVentaResponse>;

public sealed class UpdateComprobanteVentaCommandValidator : AbstractValidator<UpdateComprobanteVentaCommand>
{
    public UpdateComprobanteVentaCommandValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Data.ExecutingPlantCode).MaximumLength(2);
        RuleFor(x => x.Data.Glosa).MaximumLength(200);
        RuleFor(x => x.Data.DetractionDocument).MaximumLength(20);
        RuleFor(x => x.Data.DetractionAmount).GreaterThanOrEqualTo(0m).When(x => x.Data.DetractionAmount.HasValue);
        RuleFor(x => x.Data.CashDocument).MaximumLength(20);
        RuleForEach(x => x.Data.Detalles!).ChildRules(l =>
        {
            l.RuleFor(x => x.ProductoTipoCode).NotEmpty().MaximumLength(2);
            l.RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);
            l.RuleFor(x => x.Quantity).GreaterThan(0m);
            l.RuleFor(x => x.UnitPrice).GreaterThan(0m);
        }).When(x => x.Data.Detalles is not null);
    }
}

public sealed class UpdateComprobanteVentaCommandHandler
    : IRequestHandler<UpdateComprobanteVentaCommand, ComprobanteVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public UpdateComprobanteVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ComprobanteVentaResponse> Handle(UpdateComprobanteVentaCommand request, CancellationToken ct)
    {
        var type = ComprobanteVentaRules.NormalizeType(request.TypeCode);
        var number = ComprobanteVentaRules.NormalizeNumber(request.Number);
        var d = request.Data;
        var c = await ComprobanteVentaRules.LoadForUpdateAsync(_uow, type, number, ct);

        var structural = d.IssueDate.HasValue && d.IssueDate.Value.Date != c.IssueDate.Date || d.Detalles is not null;
        ComprobanteVentaGuias.EnsureModifiable(c, structural);

        if (d.IssueDate.HasValue && d.IssueDate.Value.Date != c.IssueDate.Date)
        {
            var date = d.IssueDate.Value.Date;
            ComprobanteVentaRules.EnsureIssueDate(date);
            await GuiaRemisionVentaRules.EnsurePeriodOpenAsync(_uow, date, ct);
            await GuiaRemisionVentaRules.EnsurePeriodOpenAsync(_uow, c.IssueDate, ct);

            c.IssueDate = date;
            c.IgvPercentage = await ComprobanteVentaRules.GetIgvPercentageAsync(_uow, date, ct);
            c.ExchangeRate = await GuiaRemisionVentaRules.GetExchangeRateAsync(_uow, c.CurrencyCode, date, ct);
        }

        if (d.Detalles is not null)
        {
            if (c.Origin != OrigenComprobanteVenta.Libre)
            {
                throw new InvalidOperationException(
                    "Las líneas de un comprobante de guías no se editan: use agregar o quitar guías.");
            }

            var fresh = await ComprobanteVentaLibreLines.BuildAsync(_uow, c, d.Detalles, ct);
            ComprobanteVentaLibreLines.Replace(c, fresh);
        }

        if (d.Glosa is not null)
        {
            c.Glosa = OrdenTrabajoVentaRules.NormalizeText(d.Glosa);
        }

        if (!string.IsNullOrWhiteSpace(d.ExecutingPlantCode))
        {
            var code = OrdenTrabajoVentaRules.NormalizeFixed(d.ExecutingPlantCode, 2);

            if (!await _uow.Comunes.Plantas.Query().AsNoTracking().AnyAsync(p => p.Code == code, ct))
            {
                throw new KeyNotFoundException($"La planta ejecutora {code} no existe.");
            }

            c.ExecutingPlantCode = code;
        }

        if (d.DetractionDocument is not null)
        {
            c.DetractionDocument = OrdenTrabajoVentaRules.NormalizeText(d.DetractionDocument);
        }

        if (d.DetractionAmount.HasValue)
        {
            c.DetractionAmount = d.DetractionAmount.Value;
        }

        if (d.CashDocument is not null)
        {
            c.CashDocument = OrdenTrabajoVentaRules.NormalizeText(d.CashDocument);
        }

        ComprobanteVentaRules.Recalc(c);
        await ComprobanteVentaRules.SaveAsync(_uow, "el comprobante", ct);

        return await ComprobanteVentaReader.GetAsync(_uow, type, number, ct);
    }
}
