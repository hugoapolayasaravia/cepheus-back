using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.CreateActividadMantenimiento
{
    public class CreateActividadMantenimientoCommandValidator : AbstractValidator<CreateActividadMantenimientoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateActividadMantenimientoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ActividadCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La actividad es obligatoria.")
                .MaximumLength(6).WithMessage("El código de actividad no puede exceder 6 caracteres.")
                .MustAsync(ActividadExists).WithMessage("La actividad no existe.");

            RuleFor(x => x.ActividadCubiertaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La actividad cubierta es obligatoria.")
                .MaximumLength(6).WithMessage("El código de actividad cubierta no puede exceder 6 caracteres.")
                .MustAsync(ActividadExists).WithMessage("La actividad cubierta no existe.");

            RuleFor(x => x.Prioridad).GreaterThanOrEqualTo(0).WithMessage("La prioridad no puede ser negativa.");

            RuleFor(x => x)
                .MustAsync(BeUnique)
                .WithMessage("Ya existe esa combinación de actividades.")
                .OverridePropertyName(nameof(CreateActividadMantenimientoCommand.ActividadCubiertaCode))
                .When(x => !string.IsNullOrWhiteSpace(x.ActividadCode) && !string.IsNullOrWhiteSpace(x.ActividadCubiertaCode));
        }

        private async Task<bool> ActividadExists(string code, CancellationToken ct)
        {
            var normalized = code.Trim().ToUpperInvariant();
            return await _uow.Mantenimiento.Maestros.Actividades.Query().AnyAsync(a => a.Code == normalized, ct);
        }

        private async Task<bool> BeUnique(CreateActividadMantenimientoCommand c, CancellationToken ct)
        {
            var act = c.ActividadCode.Trim().ToUpperInvariant();
            var cub = c.ActividadCubiertaCode.Trim().ToUpperInvariant();
            return !await _uow.Mantenimiento.Maestros.ActividadesMantenimiento.Query()
                .AnyAsync(a => a.ActividadCode == act && a.ActividadCubiertaCode == cub, ct);
        }
    }
}
