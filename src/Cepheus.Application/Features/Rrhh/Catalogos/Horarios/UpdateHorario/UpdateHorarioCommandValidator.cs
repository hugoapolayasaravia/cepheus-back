using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.UpdateHorario
{
    public class UpdateHorarioCommandValidator : AbstractValidator<UpdateHorarioCommand>
    {
        public UpdateHorarioCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del horario es obligatorio.")
                .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del horario es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}