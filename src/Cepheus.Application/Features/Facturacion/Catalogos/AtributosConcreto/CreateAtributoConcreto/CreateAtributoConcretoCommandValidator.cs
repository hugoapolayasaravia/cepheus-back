using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.CreateAtributoConcreto
{
    public class CreateAtributoConcretoCommandValidator : AbstractValidator<CreateAtributoConcretoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateAtributoConcretoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.AttributeType)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de atributo es obligatorio.")
                .Must(v => System.Enum.GetNames<TipoAtributoConcreto>().Contains(v.Trim(), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"El tipo de atributo debe ser uno de: {string.Join(", ", System.Enum.GetNames<TipoAtributoConcreto>())}.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatorio.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");
        }

    }
}
