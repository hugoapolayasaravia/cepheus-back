using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Especialidades.UpdateEspecialidad
{
    public class UpdateEspecialidadCommandValidator : AbstractValidator<UpdateEspecialidadCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateEspecialidadCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la especialidad es obligatorio.")
                .MaximumLength(1).WithMessage("El código no puede exceder 1 carácter.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la especialidad es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otra especialidad con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateEspecialidadCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Catalogos.Especialidades.Query()
                .AnyAsync(e => e.Code != command.Code && e.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
