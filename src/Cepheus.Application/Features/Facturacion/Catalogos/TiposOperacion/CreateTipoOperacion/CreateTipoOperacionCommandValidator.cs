using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.CreateTipoOperacion
{
    public class CreateTipoOperacionCommandValidator : AbstractValidator<CreateTipoOperacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoOperacionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatorio.")
                .MaximumLength(255).WithMessage("La descripción no puede exceder los 255 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un Tipo de Operación con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.TiposOperacion.Query()
                .AnyAsync(s => s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
