using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.UpdateInspeccion
{
    public class UpdateInspeccionCommandValidator : AbstractValidator<UpdateInspeccionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateInspeccionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la inspección es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la inspección es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra inspección con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateInspeccionCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Catalogos.Inspecciones.Query()
                .AnyAsync(i => i.Code != command.Code && i.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
