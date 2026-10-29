using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.UpdateActividadMantenimiento
{
    public class UpdateActividadMantenimientoCommandValidator : AbstractValidator<UpdateActividadMantenimientoCommand>
    {
        public UpdateActividadMantenimientoCommandValidator()
        {
            RuleFor(x => x.ActividadCode).NotEmpty().MaximumLength(6);
            RuleFor(x => x.ActividadCubiertaCode).NotEmpty().MaximumLength(6);
            RuleFor(x => x.Prioridad).GreaterThanOrEqualTo(0).WithMessage("La prioridad no puede ser negativa.");
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
