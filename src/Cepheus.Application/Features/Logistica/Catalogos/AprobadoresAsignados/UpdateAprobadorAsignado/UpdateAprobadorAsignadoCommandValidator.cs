using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.UpdateAprobadorAsignado
{
    public class UpdateAprobadorAsignadoCommandValidator : AbstractValidator<UpdateAprobadorAsignadoCommand>
    {
        public UpdateAprobadorAsignadoCommandValidator()
        {
            RuleFor(x => x.NivelCode).NotEmpty();
            RuleFor(x => x.TipoTransaccionCode).NotEmpty();
            RuleFor(x => x.UnidadNegocioCode).NotEmpty();
            RuleFor(x => x.MonedaCode).NotEmpty();
            RuleFor(x => x.TrabajadorCode).NotEmpty();

            RuleFor(x => x.SuplenteTrabajadorCode)
                .NotEqual(x => x.TrabajadorCode)
                .WithMessage("El suplente no puede ser la misma persona que el titular.")
                .When(x => !string.IsNullOrWhiteSpace(x.SuplenteTrabajadorCode));

            RuleFor(x => x.SuperiorTrabajadorCode)
                .NotEqual(x => x.TrabajadorCode)
                .WithMessage("El superior no puede ser la misma persona que el titular.")
                .When(x => !string.IsNullOrWhiteSpace(x.SuperiorTrabajadorCode));

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
