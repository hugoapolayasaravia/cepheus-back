using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.CreateOTResponsable
{
    public class CreateOTResponsableCommandValidator : AbstractValidator<CreateOTResponsableCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateOTResponsableCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.OrdenTrabajoCode).NotEmpty().WithMessage("La orden de trabajo es obligatoria.");
            RuleFor(x => x.TrabajadorCode).NotEmpty().WithMessage("El trabajador es obligatorio.");
            // TrabajadorCode: sin validación de existencia todavía (Trabajador aún no existe como tabla).

            RuleFor(x => x)
                .MustAsync(OrdenTrabajoExists)
                .WithMessage("La orden de trabajo indicada no existe.")
                .OverridePropertyName(nameof(CreateOTResponsableCommand.OrdenTrabajoCode));

            RuleFor(x => x.TiempoProceso).GreaterThanOrEqualTo(0).WithMessage("El tiempo de proceso no puede ser negativo.");
            RuleFor(x => x.Basico).GreaterThanOrEqualTo(0).WithMessage("El básico no puede ser negativo.");
            RuleFor(x => x.CostoTotal).GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo.");
        }

        private async Task<bool> OrdenTrabajoExists(CreateOTResponsableCommand command, CancellationToken cancellationToken)
        {
            var plantaCode = command.PlantaCode.Trim().ToUpper();
            var ordenCode = command.OrdenTrabajoCode.Trim().ToUpper();

            return await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query()
                .AnyAsync(o => o.PlantaCode == plantaCode && o.Code == ordenCode, cancellationToken);
        }
    }
}
