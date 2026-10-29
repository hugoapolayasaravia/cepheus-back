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

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.AgregarGuiasComprobanteVenta;

public sealed class AgregarGuiasComprobanteVentaRequest
{
    public List<string> Guias { get; set; } = new();
}

/// <summary>Suma guías Pendientes al comprobante (Pendiente y sin imprimir).</summary>
public sealed record AgregarGuiasComprobanteVentaCommand(string TypeCode, string Number, AgregarGuiasComprobanteVentaRequest Data)
    : IRequest<ComprobanteVentaResponse>;

public sealed class AgregarGuiasComprobanteVentaCommandValidator : AbstractValidator<AgregarGuiasComprobanteVentaCommand>
{
    public AgregarGuiasComprobanteVentaCommandValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Data.Guias).NotEmpty().WithMessage("Indique al menos una guía.");
        RuleForEach(x => x.Data.Guias).NotEmpty().MaximumLength(10);
    }
}

public sealed class AgregarGuiasComprobanteVentaCommandHandler
    : IRequestHandler<AgregarGuiasComprobanteVentaCommand, ComprobanteVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public AgregarGuiasComprobanteVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ComprobanteVentaResponse> Handle(AgregarGuiasComprobanteVentaCommand request, CancellationToken ct)
    {
        var type = ComprobanteVentaRules.NormalizeType(request.TypeCode);
        var number = ComprobanteVentaRules.NormalizeNumber(request.Number);
        var c = await ComprobanteVentaRules.LoadForUpdateAsync(_uow, type, number, ct);

        ComprobanteVentaGuias.EnsureModifiable(c, structural: true);

        if (c.Origin != OrigenComprobanteVenta.Guias)
        {
            throw new InvalidOperationException("Un comprobante libre no admite guías.");
        }

        var numbers = request.Data.Guias.Select(GuiaRemisionVentaRules.NormalizeNumber).Distinct().ToList();

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

        ComprobanteVentaRules.AttachGuias(c, guias);
        ComprobanteVentaRules.Recalc(c);
        await ComprobanteVentaRules.SaveAsync(_uow, "las guías del comprobante", ct);

        return await ComprobanteVentaReader.GetAsync(_uow, type, number, ct);
    }
}
