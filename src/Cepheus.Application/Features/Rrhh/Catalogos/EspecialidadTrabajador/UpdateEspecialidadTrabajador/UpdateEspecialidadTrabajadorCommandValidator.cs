using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Roles.UpdateRole;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Especialidades.UpdateEspecialidad
{
    public class UpdateEspecialidadTrabajadorCommandValidator : AbstractValidator<UpdateEspecialidadTrabajadorCommand>
    {
        private readonly IUnitOfWork _uow;
        public UpdateEspecialidadTrabajadorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la especialidad es obligatorio.")
                .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la especialidad es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre no puede exceder los 200 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe una especialidad con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateEspecialidadTrabajadorCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Rrhh.Catalogos.EspecialidadesTrabajador.Query()
                .AnyAsync(r => r.Name == name.Trim() && r.Code != command.Code, cancellationToken);
    }
}