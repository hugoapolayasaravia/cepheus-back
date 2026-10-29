using Cepheus.Application.Features.Rrhh.Catalogos.Epss.CreateEps;
using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.CreateEps
{
    public class CreateEpsCommandValidator : AbstractValidator<CreateEpsCommand>
    {
        public CreateEpsCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la EPS es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");
        }
    }
}