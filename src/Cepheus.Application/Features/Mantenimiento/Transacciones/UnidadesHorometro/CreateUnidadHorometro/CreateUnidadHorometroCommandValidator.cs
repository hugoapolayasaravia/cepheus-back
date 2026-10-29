using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.CreateUnidadHorometro
{
    public class CreateUnidadHorometroCommandValidator : AbstractValidator<CreateUnidadHorometroCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadHorometroCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.SubCentroCostoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La unidad (subcentro de costo) es obligatoria.")
                .MaximumLength(6).WithMessage("El código de subcentro no puede exceder 6 caracteres.")
                .MustAsync(SubCentroExists).WithMessage("La unidad (subcentro de costo) no existe.");

            RuleFor(x => x.Fecha)
                .NotEmpty().WithMessage("La fecha es obligatoria.");

            RuleFor(x => x.Horometro)
                .GreaterThanOrEqualTo(0).WithMessage("El horómetro no puede ser negativo.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("El estado del equipo no es válido.");

            RuleFor(x => x.PlantaAfectadaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta afectada es obligatoria.")
                .MaximumLength(2).WithMessage("El código de planta no puede exceder 2 caracteres.")
                .MustAsync(PlantaExists).WithMessage("La planta afectada no existe.");

            RuleFor(x => x.EquipoCode)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(8).WithMessage("El código de equipo no puede exceder 8 caracteres.")
                .MustAsync(FkExists).WithMessage("El equipo no existe.")
                .When(x => !string.IsNullOrWhiteSpace(x.EquipoCode));

            RuleFor(x => x)
                .MustAsync(BeUniqueRecord)
                .WithMessage("Ya existe un registro de esta unidad para esa fecha.")
                .OverridePropertyName(nameof(CreateUnidadHorometroCommand.Fecha))
                .When(x => !string.IsNullOrWhiteSpace(x.SubCentroCostoCode));
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
            return await _uow.Mantenimiento.Maestros.Equipos.Query().AnyAsync(f => f.Code == normalized, ct);
        }

        private async Task<bool> BeUniqueRecord(CreateUnidadHorometroCommand command, CancellationToken ct)
        {
            var scc = command.SubCentroCostoCode.Trim().ToUpperInvariant();
            var fecha = command.Fecha.Date;
            return !await _uow.Mantenimiento.Transacciones.UnidadesHorometro.Query()
                .AnyAsync(x => x.SubCentroCostoCode == scc && x.Fecha == fecha, ct);
        }
    }
}
