using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.UpdateTipoOrden
{
    public class UpdateTipoOrdenCommandValidator : AbstractValidator<UpdateTipoOrdenCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoOrdenCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de orden es obligatorio.")
                .MaximumLength(3).WithMessage("El código no puede exceder los 3 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de orden es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro tipo de orden con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateTipoOrdenCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Catalogos.TiposOrden.Query()
                .AnyAsync(t => t.Code != command.Code && t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
