using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GenerarValeDesdeNotaIngreso;

public sealed class GenerarValeDesdeNotaIngresoCommandValidator : AbstractValidator<GenerarValeDesdeNotaIngresoCommand>
{
    public GenerarValeDesdeNotaIngresoCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2).WithMessage("La planta es obligatoria.");
        RuleFor(x => x.NotaIngresoCode).NotEmpty().MaximumLength(6).WithMessage("La Nota de Ingreso es obligatoria.");
        RuleFor(x => x.FechaEntrega).NotEqual(default(DateTime)).WithMessage("La fecha de entrega es obligatoria.");
        RuleFor(x => x.SubCentroCostoCode).NotEmpty().WithMessage("El subcentro de costo beneficiado es obligatorio.");
        RuleFor(x => x.TrabajadorCode).NotEmpty().WithMessage("El trabajador es obligatorio.");
        RuleFor(x => x.UnidadNegocioCode).NotEmpty().WithMessage("La unidad de negocio es obligatoria.");
        RuleFor(x => x.PlantaAfectadaCode).NotEmpty().MaximumLength(2).WithMessage("La planta afectada es obligatoria.");
    }
}
