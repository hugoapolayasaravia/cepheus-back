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

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.QuitarGuiaComprobanteVenta;

/// <summary>Saca una guía del comprobante y la devuelve a Pendiente. Debe quedar al menos una guía (si no, anule el comprobante).</summary>
public sealed record QuitarGuiaComprobanteVentaCommand(string TypeCode, string Number, string GuiaNumber)
    : IRequest<ComprobanteVentaResponse>;

public sealed class QuitarGuiaComprobanteVentaCommandValidator : AbstractValidator<QuitarGuiaComprobanteVentaCommand>
{
    public QuitarGuiaComprobanteVentaCommandValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
        RuleFor(x => x.GuiaNumber).NotEmpty().MaximumLength(10);
    }
}

public sealed class QuitarGuiaComprobanteVentaCommandHandler
    : IRequestHandler<QuitarGuiaComprobanteVentaCommand, ComprobanteVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public QuitarGuiaComprobanteVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ComprobanteVentaResponse> Handle(QuitarGuiaComprobanteVentaCommand request, CancellationToken ct)
    {
        var type = ComprobanteVentaRules.NormalizeType(request.TypeCode);
        var number = ComprobanteVentaRules.NormalizeNumber(request.Number);
        var guiaNumber = GuiaRemisionVentaRules.NormalizeNumber(request.GuiaNumber);
        var c = await ComprobanteVentaRules.LoadForUpdateAsync(_uow, type, number, ct);

        ComprobanteVentaGuias.EnsureModifiable(c, structural: true);

        var lines = c.Detalles.Where(d => d.GuiaNumber == guiaNumber).ToList();

        if (lines.Count == 0)
        {
            throw new KeyNotFoundException($"La guía {guiaNumber} no está en el comprobante {number}.");
        }

        if (c.Detalles.Select(d => d.GuiaNumber).Where(g => g != null).Distinct().Count() <= 1)
        {
            throw new InvalidOperationException("El comprobante debe conservar al menos una guía; para liberarla anule el comprobante.");
        }

        var guias = await ComprobanteVentaGuias.LoadAsync(_uow, c, ct);
        var guia = guias.First(g => g.Number == guiaNumber);

        OrdenTrabajoVenta? ot = c.OrdenTrabajoCode is null
            ? null
            : await GuiaRemisionVentaRules.LoadOrderAsync(_uow, c.OrdenTrabajoCode, ct);

        foreach (var l in lines)
        {
            c.Detalles.Remove(l);
        }

        ComprobanteVentaGuias.Release(guia, c, ot);
        ComprobanteVentaRules.Recalc(c);
        await ComprobanteVentaRules.SaveAsync(_uow, "la guía del comprobante", ct);

        return await ComprobanteVentaReader.GetAsync(_uow, type, number, ct);
    }
}
