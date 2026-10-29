using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.CreateAprobadorAsignado
{
    public class CreateAprobadorAsignadoCommandValidator : AbstractValidator<CreateAprobadorAsignadoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateAprobadorAsignadoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.TrabajadorCode).NotEmpty().WithMessage("El trabajador (titular) es obligatorio.");
            // TrabajadorCode/Suplente/Superior: sin validación de existencia todavía (Trabajador aún no existe como tabla).

            RuleFor(x => x)
                .MustAsync(RangoAprobacionExists)
                .WithMessage("No existe un rango de aprobación para esa combinación de nivel, transacción, unidad de negocio y moneda — créalo primero.")
                .OverridePropertyName(nameof(CreateAprobadorAsignadoCommand.NivelCode));

            RuleFor(x => x)
                .MustAsync(BeUniqueAssignment)
                .WithMessage("Ese trabajador ya está asignado como aprobador para esta combinación.")
                .OverridePropertyName(nameof(CreateAprobadorAsignadoCommand.TrabajadorCode));

            RuleFor(x => x.SuplenteTrabajadorCode)
                .NotEqual(x => x.TrabajadorCode)
                .WithMessage("El suplente no puede ser la misma persona que el titular.")
                .When(x => !string.IsNullOrWhiteSpace(x.SuplenteTrabajadorCode));

            RuleFor(x => x.SuperiorTrabajadorCode)
                .NotEqual(x => x.TrabajadorCode)
                .WithMessage("El superior no puede ser la misma persona que el titular.")
                .When(x => !string.IsNullOrWhiteSpace(x.SuperiorTrabajadorCode));
        }

        private async Task<bool> RangoAprobacionExists(CreateAprobadorAsignadoCommand command, CancellationToken ct)
        {
            var nivel = command.NivelCode.Trim().ToUpper();
            var trans = command.TipoTransaccionCode.Trim().ToUpper();
            var une = command.UnidadNegocioCode.Trim().ToUpper();
            var mon = command.MonedaCode.Trim().ToUpper();

            return await _uow.Logistica.Catalogos.RangosAprobacion.Query()
                .AnyAsync(r => r.NivelCode == nivel && r.TipoTransaccionCode == trans &&
                               r.UnidadNegocioCode == une && r.MonedaCode == mon, ct);
        }

        private async Task<bool> BeUniqueAssignment(CreateAprobadorAsignadoCommand command, CancellationToken ct)
        {
            var nivel = command.NivelCode.Trim().ToUpper();
            var trans = command.TipoTransaccionCode.Trim().ToUpper();
            var une = command.UnidadNegocioCode.Trim().ToUpper();
            var mon = command.MonedaCode.Trim().ToUpper();
            var trabajador = command.TrabajadorCode.Trim().ToUpper();

            return !await _uow.Logistica.Catalogos.AprobadoresAsignados.Query()
                .AnyAsync(a => a.NivelCode == nivel && a.TipoTransaccionCode == trans &&
                               a.UnidadNegocioCode == une && a.MonedaCode == mon &&
                               a.TrabajadorCode == trabajador, ct);
        }
    }
}
