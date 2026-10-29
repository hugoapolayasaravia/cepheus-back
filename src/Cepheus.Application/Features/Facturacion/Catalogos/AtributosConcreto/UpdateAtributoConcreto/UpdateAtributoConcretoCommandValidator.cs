using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.UpdateAtributoConcreto
{
    public class UpdateAtributoConcretoCommandValidator : AbstractValidator<UpdateAtributoConcretoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAtributoConcretoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .MaximumLength(4).WithMessage("El código no puede exceder los 4 caracteres.");

            RuleFor(x => x.AttributeType)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de atributo es obligatorio.")
                .Must(v => System.Enum.GetNames<TipoAtributoConcreto>().Contains(v.Trim(), StringComparer.OrdinalIgnoreCase))
                .WithMessage($"El tipo de atributo debe ser uno de: {string.Join(", ", System.Enum.GetNames<TipoAtributoConcreto>())}.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatorio.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
