using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.UpdateElementoConcreto
{
    public class UpdateElementoConcretoCommandValidator : AbstractValidator<UpdateElementoConcretoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateElementoConcretoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del elemento de concreto es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del elemento de concreto es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro elemento de concreto con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateElementoConcretoCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.ElementosConcreto.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
