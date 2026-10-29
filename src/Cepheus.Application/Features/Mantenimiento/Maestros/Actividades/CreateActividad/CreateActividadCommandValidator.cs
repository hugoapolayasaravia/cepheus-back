using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.CreateActividad
{
    public class CreateActividadCommandValidator : AbstractValidator<CreateActividadCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateActividadCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.VerboActividadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El verbo de actividad es obligatorio.")
                .MustAsync(VerboExists).WithMessage("El verbo de actividad indicado no existe.");

            RuleFor(x => x.ObjetoActividadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El objeto de actividad es obligatorio.")
                .MustAsync(ObjetoExists).WithMessage("El objeto de actividad indicado no existe.");

            RuleFor(x => x)
                .MustAsync(BeUniqueCombination)
                .WithMessage("Ya existe una actividad con esa combinación de verbo y objeto.")
                .OverridePropertyName(nameof(CreateActividadCommand.VerboActividadCode));
        }

        private async Task<bool> VerboExists(string code, CancellationToken cancellationToken)
            => await _uow.Mantenimiento.Maestros.VerbosActividad.Query()
                .AnyAsync(v => v.Code == code.Trim().ToUpper(), cancellationToken);

        private async Task<bool> ObjetoExists(string code, CancellationToken cancellationToken)
            => await _uow.Mantenimiento.Maestros.ObjetosActividad.Query()
                .AnyAsync(o => o.Code == code.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueCombination(CreateActividadCommand command, CancellationToken cancellationToken)
        {
            var code = command.VerboActividadCode.Trim().ToUpper() + command.ObjetoActividadCode.Trim().ToUpper();

            return !await _uow.Mantenimiento.Maestros.Actividades.Query()
                .AnyAsync(a => a.Code == code, cancellationToken);
        }
    }
}
