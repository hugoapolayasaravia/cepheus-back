using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.CreatePlanPreventivoManoObra
{
    public class CreatePlanPreventivoManoObraCommandValidator : AbstractValidator<CreatePlanPreventivoManoObraCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreatePlanPreventivoManoObraCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.PlanCode).NotEmpty().MaximumLength(6);

            RuleFor(x => x)
                .MustAsync(PlanExists)
                .WithMessage("El plan preventivo no existe.")
                .OverridePropertyName(nameof(CreatePlanPreventivoManoObraCommand.PlanCode))
                .When(x => !string.IsNullOrWhiteSpace(x.PlantaCode) && !string.IsNullOrWhiteSpace(x.PlanCode));

            RuleFor(x => x.TrabajadorCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El trabajador es obligatorio.")
                .MaximumLength(5).WithMessage("El código no puede exceder 5 caracteres.")
                .MustAsync(KeyExists).WithMessage("El trabajador no existe.");

            RuleFor(x => x.Tiempo).GreaterThanOrEqualTo(0).WithMessage("El tiempo no puede ser negativo.");
            RuleFor(x => x.CostoTotal).GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo.");

            RuleFor(x => x)
                .MustAsync(BeUnique)
                .WithMessage("Ya está registrado en este plan.")
                .OverridePropertyName(nameof(CreatePlanPreventivoManoObraCommand.TrabajadorCode))
                .When(x => !string.IsNullOrWhiteSpace(x.PlantaCode) && !string.IsNullOrWhiteSpace(x.PlanCode)
                           && !string.IsNullOrWhiteSpace(x.TrabajadorCode));
        }

        private async Task<bool> PlanExists(CreatePlanPreventivoManoObraCommand c, CancellationToken ct)
        {
            var planta = c.PlantaCode.Trim().ToUpperInvariant();
            var plan = c.PlanCode.Trim().ToUpperInvariant();
            return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                .AnyAsync(p => p.PlantaCode == planta && p.Code == plan, ct);
        }

        private async Task<bool> KeyExists(string code, CancellationToken ct)
        {
            var normalized = code.Trim().ToUpperInvariant();
            return await _uow.Rrhh.Maestros.Trabajadores.Query().AnyAsync(e => e.Code == normalized, ct);
        }

        private async Task<bool> BeUnique(CreatePlanPreventivoManoObraCommand c, CancellationToken ct)
        {
            var planta = c.PlantaCode.Trim().ToUpperInvariant();
            var plan = c.PlanCode.Trim().ToUpperInvariant();
            var key = c.TrabajadorCode.Trim().ToUpperInvariant();
            return !await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoManoObra.Query()
                .AnyAsync(d => d.PlantaCode == planta && d.PlanCode == plan && d.TrabajadorCode == key, ct);
        }
    }
}
