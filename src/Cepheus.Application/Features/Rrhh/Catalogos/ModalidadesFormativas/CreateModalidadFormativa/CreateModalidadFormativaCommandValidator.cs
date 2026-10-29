using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.CreateModalidadFormativa
{
    public class CreateModalidadFormativaCommandValidator : AbstractValidator<CreateModalidadFormativaCommand>
    {
        public CreateModalidadFormativaCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la modalidad formativa es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}