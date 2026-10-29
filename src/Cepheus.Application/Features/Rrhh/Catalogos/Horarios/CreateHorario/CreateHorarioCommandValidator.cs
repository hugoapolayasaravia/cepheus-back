using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Horarios.CreateHorario
{
    public class CreateHorarioCommandValidator : AbstractValidator<CreateHorarioCommand>
    {
        public CreateHorarioCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del horario es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}