using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>Reglas de formato de la cabecera, compartidas por Create y Update.</summary>
public abstract class OrdenTrabajoVentaFieldsValidator<T> : AbstractValidator<T> where T : OrdenTrabajoVentaUpdateFields
{
    protected OrdenTrabajoVentaFieldsValidator()
    {
        RuleFor(x => x.DispatchDate).NotEmpty().WithMessage("La fecha de despacho es obligatoria.");

        RuleFor(x => x.DispatchTime).NotEmpty()
            .Must(OrdenTrabajoVentaRules.IsValidTime)
            .WithMessage("La hora de despacho debe tener formato HH:mm (hora 00-23, minuto 00-59).");

        RuleFor(x => x.VendedorCode).MaximumLength(4);

        RuleFor(x => x.MotivoCode).NotEmpty().MaximumLength(10)
            .WithMessage("El motivo es obligatorio.");

        RuleFor(x => x.CurrencyCode).MaximumLength(1);
        RuleFor(x => x.Observations).MaximumLength(4000);
        RuleFor(x => x.CutSheetObservations).MaximumLength(4000);
    }
}

public sealed class OrdenTrabajoVentaDetalleRequestValidator : AbstractValidator<OrdenTrabajoVentaDetalleRequest>
{
    public OrdenTrabajoVentaDetalleRequestValidator()
    {
        RuleFor(x => x.ProductoTipoCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.NegocioCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("El precio debe ser mayor a cero.");
    }
}

public sealed class UpdateOrdenTrabajoVentaDetalleRequestValidator : AbstractValidator<UpdateOrdenTrabajoVentaDetalleRequest>
{
    public UpdateOrdenTrabajoVentaDetalleRequestValidator()
    {
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("El precio debe ser mayor a cero.");
    }
}
