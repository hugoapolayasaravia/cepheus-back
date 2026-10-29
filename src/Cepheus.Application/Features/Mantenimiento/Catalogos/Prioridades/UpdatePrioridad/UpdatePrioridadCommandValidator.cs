using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.UpdatePrioridad
{
    public class UpdatePrioridadCommandValidator : AbstractValidator<UpdatePrioridadCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePrioridadCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la prioridad es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la prioridad es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra prioridad con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdatePrioridadCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Catalogos.Prioridades.Query()
                .AnyAsync(p => p.Code != command.Code && p.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
