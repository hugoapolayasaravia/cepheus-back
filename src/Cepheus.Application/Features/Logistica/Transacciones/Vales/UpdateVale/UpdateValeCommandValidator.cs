using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.UpdateVale;

public sealed class UpdateValeCommandValidator : AbstractValidator<UpdateValeCommand>
{
    public UpdateValeCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(7);
        RuleFor(x => x.FechaEntrega).NotEmpty()
            .WithMessage("La fecha de entrega es obligatoria.");
        RuleFor(x => x.UnidadNegocioCode).NotEmpty().MaximumLength(6)
            .WithMessage("La unidad de negocio es obligatoria.");
        RuleFor(x => x.SubCentroCostoCode).MaximumLength(6);
        RuleFor(x => x.SubCentroEjecutorCode).MaximumLength(4);
        RuleFor(x => x.TrabajadorCode).MaximumLength(5);
        RuleFor(x => x.PlantaAfectadaCode).MaximumLength(2);
    }
}
