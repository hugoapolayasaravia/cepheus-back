using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.UpdateCentroEjecutor
{
    public class UpdateCentroEjecutorCommandValidator : AbstractValidator<UpdateCentroEjecutorCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCentroEjecutorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del centro ejecutor es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del centro ejecutor es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro centro ejecutor con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateCentroEjecutorCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Maestros.CentrosEjecutores.Query()
                .AnyAsync(c => c.Code != command.Code && c.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
