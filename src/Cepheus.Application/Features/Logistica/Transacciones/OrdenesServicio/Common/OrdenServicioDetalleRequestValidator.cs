using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;

/// <summary>Validación de forma de una línea. Las validaciones contra maestros están en OrdenServicioRules.</summary>
public sealed class OrdenServicioDetalleRequestValidator : AbstractValidator<OrdenServicioDetalleRequest>
{
    public OrdenServicioDetalleRequestValidator()
    {
        RuleFor(x => x.ArticuloCode)
            .NotEmpty().WithMessage("El artículo es obligatorio.")
            .MaximumLength(7).WithMessage("El código de artículo no puede exceder 7 caracteres.");

        RuleFor(x => x.Glosa)
            .MaximumLength(4000).WithMessage("La glosa no puede exceder 4000 caracteres.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.")
            .LessThan(10_000_000_000m).WithMessage("La cantidad excede el máximo permitido.")
            .Must(c => decimal.Round(c, 2) == c).WithMessage("La cantidad admite como máximo 2 decimales.");

        RuleFor(x => x.Precio)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a cero.")
            .LessThan(1_000_000_000_000m).WithMessage("El precio excede el máximo permitido.")
            .Must(p => decimal.Round(p, 6) == p).WithMessage("El precio admite como máximo 6 decimales.");

        RuleFor(x => x.Descuento)
            .InclusiveBetween(0, 100).WithMessage("El descuento es un porcentaje entre 0 y 100.")
            .Must(d => decimal.Round(d, 2) == d).WithMessage("El descuento admite como máximo 2 decimales.");

        RuleFor(x => x.TipoValeCode)
            .NotEmpty().WithMessage("El tipo de vale es obligatorio.")
            .MaximumLength(3).WithMessage("El código de tipo de vale no puede exceder 3 caracteres.");

        RuleFor(x => x.SubCentroCostoCode).MaximumLength(6)
            .WithMessage("El código de sub centro de costo no puede exceder 6 caracteres.");
        RuleFor(x => x.SubCentroEjecutorCode).MaximumLength(4)
            .WithMessage("El código de sub centro ejecutor no puede exceder 4 caracteres.");
        RuleFor(x => x.OrdenTrabajoCode).MaximumLength(6)
            .WithMessage("El código de orden de trabajo no puede exceder 6 caracteres.");
    }
}
