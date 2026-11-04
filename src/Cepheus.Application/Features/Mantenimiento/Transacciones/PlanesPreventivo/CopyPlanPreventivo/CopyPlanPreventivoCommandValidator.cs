using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.CopyPlanPreventivo
{
    public class CopyPlanPreventivoCommandValidator : AbstractValidator<CopyPlanPreventivoCommand>
    {
        public CopyPlanPreventivoCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
        }
    }
}
