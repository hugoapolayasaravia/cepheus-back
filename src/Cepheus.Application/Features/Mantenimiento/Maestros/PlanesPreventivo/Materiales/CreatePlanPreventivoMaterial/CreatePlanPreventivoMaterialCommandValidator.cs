using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.CreatePlanPreventivoMaterial
{
    public class CreatePlanPreventivoMaterialCommandValidator : AbstractValidator<CreatePlanPreventivoMaterialCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreatePlanPreventivoMaterialCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.PlanCode).NotEmpty().MaximumLength(6);

            RuleFor(x => x)
                .MustAsync(PlanExists)
                .WithMessage("El plan preventivo no existe.")
                .OverridePropertyName(nameof(CreatePlanPreventivoMaterialCommand.PlanCode))
                .When(x => !string.IsNullOrWhiteSpace(x.PlantaCode) && !string.IsNullOrWhiteSpace(x.PlanCode));

            RuleFor(x => x.ArticuloCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El artículo es obligatorio.")
                .MaximumLength(7).WithMessage("El código no puede exceder 7 caracteres.")
                .MustAsync(KeyExists).WithMessage("El artículo no existe.");

            RuleFor(x => x.Cantidad).GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
            RuleFor(x => x.CostoUnitario).GreaterThanOrEqualTo(0).WithMessage("El costo unitario no puede ser negativo.");
            RuleFor(x => x.CostoTotal).GreaterThanOrEqualTo(0).WithMessage("El costo total no puede ser negativo.");

            RuleFor(x => x)
                .MustAsync(BeUnique)
                .WithMessage("Ya está registrado en este plan.")
                .OverridePropertyName(nameof(CreatePlanPreventivoMaterialCommand.ArticuloCode))
                .When(x => !string.IsNullOrWhiteSpace(x.PlantaCode) && !string.IsNullOrWhiteSpace(x.PlanCode)
                           && !string.IsNullOrWhiteSpace(x.ArticuloCode));
        }

        private async Task<bool> PlanExists(CreatePlanPreventivoMaterialCommand c, CancellationToken ct)
        {
            var planta = c.PlantaCode.Trim().ToUpperInvariant();
            var plan = c.PlanCode.Trim().ToUpperInvariant();
            return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                .AnyAsync(p => p.PlantaCode == planta && p.Code == plan, ct);
        }

        private async Task<bool> KeyExists(string code, CancellationToken ct)
        {
            var normalized = code.Trim().ToUpperInvariant();
            return await _uow.Logistica.Maestros.Articulos.Query().AnyAsync(e => e.Code == normalized, ct);
        }

        private async Task<bool> BeUnique(CreatePlanPreventivoMaterialCommand c, CancellationToken ct)
        {
            var planta = c.PlantaCode.Trim().ToUpperInvariant();
            var plan = c.PlanCode.Trim().ToUpperInvariant();
            var key = c.ArticuloCode.Trim().ToUpperInvariant();
            return !await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoMateriales.Query()
                .AnyAsync(d => d.PlantaCode == planta && d.PlanCode == plan && d.ArticuloCode == key, ct);
        }
    }
}
