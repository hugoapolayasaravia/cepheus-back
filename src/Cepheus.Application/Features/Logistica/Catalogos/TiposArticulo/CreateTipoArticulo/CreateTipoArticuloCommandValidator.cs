using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.CreateTipoArticulo
{
    public class CreateTipoArticuloCommandValidator : AbstractValidator<CreateTipoArticuloCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoArticuloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de artículo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de artículo con ese nombre.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La Descripción del tipo de artículo es obligatorio.")
                .MaximumLength(500).WithMessage("La Descripción  no puede exceder los 500 caracteres.");

        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.TiposArticulo.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}