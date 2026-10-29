using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.CreateVerboActividad
{
    public class CreateVerboActividadCommandValidator : AbstractValidator<CreateVerboActividadCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateVerboActividadCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del verbo de actividad es obligatorio.")
                .MaximumLength(3).WithMessage("El código no puede exceder los 3 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un verbo de actividad con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del verbo de actividad es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un verbo de actividad con ese nombre.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            var normalizedCode = code.Trim().ToUpper();

            return !await _uow.Mantenimiento.Maestros.VerbosActividad.Query()
                .AnyAsync(v => v.Code.ToUpper() == normalizedCode, cancellationToken);
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Maestros.VerbosActividad.Query()
                .AnyAsync(v => v.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
