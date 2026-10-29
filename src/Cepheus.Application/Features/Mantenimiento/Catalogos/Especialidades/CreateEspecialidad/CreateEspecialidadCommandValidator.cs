using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.CreateEspecialidad
{
    public class CreateEspecialidadCommandValidator : AbstractValidator<CreateEspecialidadCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateEspecialidadCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código de la especialidad es obligatorio.")
                .MaximumLength(1).WithMessage("El código no puede exceder 1 carácter.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe una especialidad con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la especialidad es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una especialidad con ese nombre.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            var normalizedCode = code.Trim().ToUpper();

            return !await _uow.Mantenimiento.Catalogos.Especialidades.Query()
                .AnyAsync(e => e.Code.ToUpper() == normalizedCode, cancellationToken);
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Catalogos.Especialidades.Query()
                .AnyAsync(e => e.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
