using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.CreateCentroEjecutor
{
    public class CreateCentroEjecutorCommandValidator : AbstractValidator<CreateCentroEjecutorCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCentroEjecutorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del centro ejecutor es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un centro ejecutor con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del centro ejecutor es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un centro ejecutor con ese nombre.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            var normalizedCode = code.Trim().ToUpper();

            return !await _uow.Mantenimiento.Maestros.CentrosEjecutores.Query()
                .AnyAsync(c => c.Code.ToUpper() == normalizedCode, cancellationToken);
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Maestros.CentrosEjecutores.Query()
                .AnyAsync(c => c.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
