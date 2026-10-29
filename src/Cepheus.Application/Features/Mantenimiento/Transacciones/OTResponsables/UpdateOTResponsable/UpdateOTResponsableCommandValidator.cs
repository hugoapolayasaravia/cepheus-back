using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.UpdateOTResponsable
{
    public class UpdateOTResponsableCommandValidator : AbstractValidator<UpdateOTResponsableCommand>
    {
        public UpdateOTResponsableCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.OrdenTrabajoCode).NotEmpty();
            RuleFor(x => x.TrabajadorCode).NotEmpty();
            RuleFor(x => x.FechaProceso).NotEmpty();

            RuleFor(x => x.TiempoProceso).GreaterThanOrEqualTo(0).WithMessage("El tiempo de proceso no puede ser negativo.");
            RuleFor(x => x.Basico).GreaterThanOrEqualTo(0).WithMessage("El básico no puede ser negativo.");
            RuleFor(x => x.CostoTotal).GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
