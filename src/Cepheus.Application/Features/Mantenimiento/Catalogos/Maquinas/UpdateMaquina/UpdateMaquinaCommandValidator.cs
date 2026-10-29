using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.UpdateMaquina
{
    public class UpdateMaquinaCommandValidator : AbstractValidator<UpdateMaquinaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateMaquinaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la máquina es obligatorio.")
                .MaximumLength(4).WithMessage("El código no puede exceder los 4 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la máquina es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra máquina con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateMaquinaCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Catalogos.Maquinas.Query()
                .AnyAsync(m => m.Code != command.Code && m.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
