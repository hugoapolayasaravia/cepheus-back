using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.CreateMaquina
{
    public class CreateMaquinaCommandValidator : AbstractValidator<CreateMaquinaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateMaquinaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código de la máquina es obligatorio.")
                .MaximumLength(4).WithMessage("El código no puede exceder los 4 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe una máquina con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la máquina es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una máquina con ese nombre.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            var normalizedCode = code.Trim().ToUpper();

            return !await _uow.Mantenimiento.Catalogos.Maquinas.Query()
                .AnyAsync(m => m.Code.ToUpper() == normalizedCode, cancellationToken);
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Catalogos.Maquinas.Query()
                .AnyAsync(m => m.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
