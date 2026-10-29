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

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.MarcarImpresaComprobanteVenta;

/// <summary>Marca el comprobante como impreso: desde ahí no se cambian sus líneas, guías ni fecha.</summary>
public sealed record MarcarImpresaComprobanteVentaCommand(string TypeCode, string Number) : IRequest<ComprobanteVentaResponse>;

public sealed class MarcarImpresaComprobanteVentaCommandValidator : AbstractValidator<MarcarImpresaComprobanteVentaCommand>
{
    public MarcarImpresaComprobanteVentaCommandValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
    }
}

public sealed class MarcarImpresaComprobanteVentaCommandHandler
    : IRequestHandler<MarcarImpresaComprobanteVentaCommand, ComprobanteVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public MarcarImpresaComprobanteVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ComprobanteVentaResponse> Handle(MarcarImpresaComprobanteVentaCommand request, CancellationToken ct)
    {
        var type = ComprobanteVentaRules.NormalizeType(request.TypeCode);
        var number = ComprobanteVentaRules.NormalizeNumber(request.Number);
        var c = await ComprobanteVentaRules.LoadForUpdateAsync(_uow, type, number, ct);

        if (c.Status == EstadoComprobanteVenta.Anulado)
        {
            throw new InvalidOperationException($"El comprobante {number} está anulado; no se imprime.");
        }

        if (!c.IsPrinted)
        {
            c.IsPrinted = true;
            c.PrintedAt = DateTime.Now;
            await ComprobanteVentaRules.SaveAsync(_uow, "la impresión del comprobante", ct);
        }

        return await ComprobanteVentaReader.GetAsync(_uow, type, number, ct);
    }
}
