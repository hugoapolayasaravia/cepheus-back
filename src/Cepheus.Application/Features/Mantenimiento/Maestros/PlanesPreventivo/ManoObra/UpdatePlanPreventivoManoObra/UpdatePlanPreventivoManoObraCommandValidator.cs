using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.UpdatePlanPreventivoManoObra
{
    public class UpdatePlanPreventivoManoObraCommandValidator : AbstractValidator<UpdatePlanPreventivoManoObraCommand>
    {
        public UpdatePlanPreventivoManoObraCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.PlanCode).NotEmpty().MaximumLength(6);
            RuleFor(x => x.TrabajadorCode).NotEmpty().MaximumLength(5);

            RuleFor(x => x.Tiempo).GreaterThanOrEqualTo(0).WithMessage("El tiempo no puede ser negativo.");
            RuleFor(x => x.CostoTotal).GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
