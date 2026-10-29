using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.Programaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.GuiasRemisionVenta.MarcarImpresaGuiaRemisionVenta;

/// <summary>Marca la guía como impresa (Impresion_gve = 'S'): desde ahí ya no se puede modificar.</summary>
public sealed record MarcarImpresaGuiaRemisionVentaCommand(string Number) : IRequest<GuiaRemisionVentaResponse>;

public sealed class MarcarImpresaGuiaRemisionVentaCommandValidator : AbstractValidator<MarcarImpresaGuiaRemisionVentaCommand>
{
    public MarcarImpresaGuiaRemisionVentaCommandValidator() => RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
}

public sealed class MarcarImpresaGuiaRemisionVentaCommandHandler
    : IRequestHandler<MarcarImpresaGuiaRemisionVentaCommand, GuiaRemisionVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public MarcarImpresaGuiaRemisionVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<GuiaRemisionVentaResponse> Handle(MarcarImpresaGuiaRemisionVentaCommand request, CancellationToken ct)
    {
        var number = GuiaRemisionVentaRules.NormalizeNumber(request.Number);
        var g = await GuiaRemisionVentaReader.LoadForUpdateAsync(_uow, number, ct);

        if (g.Status == EstadoGuiaRemisionVenta.Anulado)
        {
            throw new InvalidOperationException($"La guía {number} está anulada; no se imprime.");
        }

        if (!g.IsPrinted)
        {
            g.IsPrinted = true;
            g.PrintedAt = DateTime.Now;
            await GuiaRemisionVentaRules.SaveAsync(_uow, "la impresión de la guía", ct);
        }

        return await GuiaRemisionVentaReader.GetAsync(_uow, number, ct);
    }
}
