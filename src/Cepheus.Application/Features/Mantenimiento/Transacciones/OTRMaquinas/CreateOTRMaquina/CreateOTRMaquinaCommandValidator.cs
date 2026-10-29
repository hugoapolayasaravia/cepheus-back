using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.CreateOTRMaquina
{
    public class CreateOTRMaquinaCommandValidator : AbstractValidator<CreateOTRMaquinaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateOTRMaquinaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.OrdenTrabajoCode).NotEmpty().WithMessage("La orden de trabajo es obligatoria.");

            RuleFor(x => x.MaquinaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La máquina es obligatoria.")
                .MustAsync(MaquinaExists).WithMessage("La máquina indicada no existe.");

            RuleFor(x => x)
                .MustAsync(OrdenTrabajoExists)
                .WithMessage("La orden de trabajo indicada no existe.")
                .OverridePropertyName(nameof(CreateOTRMaquinaCommand.OrdenTrabajoCode));

            RuleFor(x => x)
                .MustAsync(BeUniqueCombination)
                .WithMessage("Ya existe un registro de esta máquina para la orden de trabajo indicada; actualice el existente en vez de crear uno nuevo.")
                .OverridePropertyName(nameof(CreateOTRMaquinaCommand.MaquinaCode));

            RuleFor(x => x.Cantidad).GreaterThanOrEqualTo(0).WithMessage("La cantidad no puede ser negativa.");
            RuleFor(x => x.Horas).GreaterThanOrEqualTo(0).WithMessage("Las horas no pueden ser negativas.");
        }

        private async Task<bool> MaquinaExists(string code, CancellationToken ct)
            => await _uow.Mantenimiento.Catalogos.Maquinas.Query().AnyAsync(m => m.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> OrdenTrabajoExists(CreateOTRMaquinaCommand command, CancellationToken ct)
        {
            var plantaCode = command.PlantaCode.Trim().ToUpper();
            var ordenCode = command.OrdenTrabajoCode.Trim().ToUpper();

            return await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query()
                .AnyAsync(o => o.PlantaCode == plantaCode && o.Code == ordenCode, ct);
        }

        private async Task<bool> BeUniqueCombination(CreateOTRMaquinaCommand command, CancellationToken ct)
        {
            var plantaCode = command.PlantaCode.Trim().ToUpper();
            var ordenCode = command.OrdenTrabajoCode.Trim().ToUpper();
            var maquinaCode = command.MaquinaCode.Trim().ToUpper();

            return !await _uow.Mantenimiento.Transacciones.OTRMaquinas.Query()
                .AnyAsync(o => o.PlantaCode == plantaCode && o.OrdenTrabajoCode == ordenCode && o.MaquinaCode == maquinaCode, ct);
        }
    }
}
