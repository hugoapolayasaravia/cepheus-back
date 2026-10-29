using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.UpdateObjetoActividad
{
    public class UpdateObjetoActividadCommandValidator : AbstractValidator<UpdateObjetoActividadCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateObjetoActividadCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del objeto de actividad es obligatorio.")
                .MaximumLength(3).WithMessage("El código no puede exceder los 3 caracteres.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del objeto de actividad es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe otro objeto de actividad con ese nombre.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> BeUniqueName(UpdateObjetoActividadCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Mantenimiento.Maestros.ObjetosActividad.Query()
                .AnyAsync(o => o.Code != command.Code && o.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
