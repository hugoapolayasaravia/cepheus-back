using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.UpdateTipoArticulo
{
    public class UpdateTipoArticuloCommandValidator : AbstractValidator<UpdateTipoArticuloCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoArticuloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de artículo es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de artículo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro tipo de artículo con ese nombre.");

            RuleFor(x => x.Description)
             .Cascade(CascadeMode.Stop)
             .NotEmpty().WithMessage("La Descripción del tipo de artículo es obligatorio.")
             .MaximumLength(500).WithMessage("La Descripción  no puede exceder los 500 caracteres.");


            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateTipoArticuloCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.TiposArticulo.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}