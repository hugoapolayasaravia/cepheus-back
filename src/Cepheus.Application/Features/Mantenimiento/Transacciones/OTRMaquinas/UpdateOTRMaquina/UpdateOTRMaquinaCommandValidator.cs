using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.UpdateOTRMaquina
{
    public class UpdateOTRMaquinaCommandValidator : AbstractValidator<UpdateOTRMaquinaCommand>
    {
        public UpdateOTRMaquinaCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.OrdenTrabajoCode).NotEmpty();
            RuleFor(x => x.MaquinaCode).NotEmpty();
            RuleFor(x => x.FechaProceso).NotEmpty();

            RuleFor(x => x.Cantidad).GreaterThanOrEqualTo(0).WithMessage("La cantidad no puede ser negativa.");
            RuleFor(x => x.Horas).GreaterThanOrEqualTo(0).WithMessage("Las horas no pueden ser negativas.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
