using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.CreateCargo
{
    public class CreateCargoCommandValidator : AbstractValidator<CreateCargoCommand>
    {
        public CreateCargoCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del cargo es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}