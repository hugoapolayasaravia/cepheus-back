using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.UpdatePlanPreventivoMaterial
{
    public class UpdatePlanPreventivoMaterialCommandValidator : AbstractValidator<UpdatePlanPreventivoMaterialCommand>
    {
        public UpdatePlanPreventivoMaterialCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.PlanCode).NotEmpty().MaximumLength(6);
            RuleFor(x => x.ArticuloCode).NotEmpty().MaximumLength(7);

            RuleFor(x => x.Cantidad).GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
            RuleFor(x => x.CostoUnitario).GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
            RuleFor(x => x.CostoTotal).GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
