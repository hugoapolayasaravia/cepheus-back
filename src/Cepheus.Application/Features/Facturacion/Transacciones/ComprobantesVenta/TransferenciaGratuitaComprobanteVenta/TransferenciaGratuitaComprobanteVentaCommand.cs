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

namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.TransferenciaGratuitaComprobanteVenta;

public sealed class TransferenciaGratuitaComprobanteVentaRequest
{
    /// <summary>true = transferencia gratuita (PB tipo 'R'); false = la quita.</summary>
    public bool IsFreeTransfer { get; set; }
}

/// <summary>Marca o desmarca el comprobante Pendiente como transferencia gratuita; la glosa es obligatoria al marcar.</summary>
public sealed record TransferenciaGratuitaComprobanteVentaCommand(
    string TypeCode, string Number, TransferenciaGratuitaComprobanteVentaRequest Data) : IRequest<ComprobanteVentaResponse>;

public sealed class TransferenciaGratuitaComprobanteVentaCommandValidator
    : AbstractValidator<TransferenciaGratuitaComprobanteVentaCommand>
{
    public TransferenciaGratuitaComprobanteVentaCommandValidator()
    {
        RuleFor(x => x.TypeCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
    }
}

public sealed class TransferenciaGratuitaComprobanteVentaCommandHandler
    : IRequestHandler<TransferenciaGratuitaComprobanteVentaCommand, ComprobanteVentaResponse>
{
    private readonly IUnitOfWork _uow;

    public TransferenciaGratuitaComprobanteVentaCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ComprobanteVentaResponse> Handle(TransferenciaGratuitaComprobanteVentaCommand request, CancellationToken ct)
    {
        var type = ComprobanteVentaRules.NormalizeType(request.TypeCode);
        var number = ComprobanteVentaRules.NormalizeNumber(request.Number);
        var c = await ComprobanteVentaRules.LoadForUpdateAsync(_uow, type, number, ct);

        ComprobanteVentaGuias.EnsureModifiable(c, structural: false);

        if (request.Data.IsFreeTransfer && string.IsNullOrWhiteSpace(c.Glosa))
        {
            throw new InvalidOperationException("Para una transferencia gratuita la glosa es obligatoria (indíquela al modificar el comprobante).");
        }

        c.IsFreeTransfer = request.Data.IsFreeTransfer;
        await ComprobanteVentaRules.SaveAsync(_uow, "el comprobante", ct);

        return await ComprobanteVentaReader.GetAsync(_uow, type, number, ct);
    }
}
