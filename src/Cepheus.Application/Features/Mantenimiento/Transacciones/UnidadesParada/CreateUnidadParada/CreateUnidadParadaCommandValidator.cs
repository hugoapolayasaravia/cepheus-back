using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.CreateUnidadParada
{
    public class CreateUnidadParadaCommandValidator : AbstractValidator<CreateUnidadParadaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadParadaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.SubCentroCostoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La unidad (subcentro de costo) es obligatoria.")
                .MaximumLength(6).WithMessage("El código de subcentro no puede exceder 6 caracteres.")
                .MustAsync(SubCentroExists).WithMessage("La unidad (subcentro de costo) no existe.");

            RuleFor(x => x.Fecha)
                .NotEmpty().WithMessage("La fecha es obligatoria.");

            // 0 es válido: el SP legado no registra nada cuando Horas = 0 (ver CreateUnidadParadaCommand).
            RuleFor(x => x.Horas)
                .GreaterThanOrEqualTo(0).WithMessage("Las horas de parada no pueden ser negativas.")
                .LessThanOrEqualTo(12).WithMessage("Las horas de parada no pueden ser mayores a 12 por día.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("El estado del equipo no es válido.");

            RuleFor(x => x.PlantaAfectadaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta afectada es obligatoria.")
                .MaximumLength(2).WithMessage("El código de planta no puede exceder 2 caracteres.")
                .MustAsync(PlantaExists).WithMessage("La planta afectada no existe.");

            RuleFor(x => x.FallaCode)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(5).WithMessage("El código de falla no puede exceder 5 caracteres.")
                .MustAsync(FkExists).WithMessage("La falla no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.FallaCode));
        }

        private async Task<bool> SubCentroExists(string code, CancellationToken ct)
        {
            var normalized = code.Trim().ToUpperInvariant();
            return await _uow.Logistica.Maestros.SubCentrosCosto.Query().AnyAsync(s => s.Code == normalized, ct);
        }

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
        {
            var normalized = code.Trim().ToUpperInvariant();
            return await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == normalized, ct);
        }

        private async Task<bool> FkExists(string? code, CancellationToken ct)
        {
            var normalized = code!.Trim().ToUpperInvariant();
            return await _uow.Mantenimiento.Catalogos.Fallas.Query().AnyAsync(f => f.Code == normalized, ct);
        }
    }
}
