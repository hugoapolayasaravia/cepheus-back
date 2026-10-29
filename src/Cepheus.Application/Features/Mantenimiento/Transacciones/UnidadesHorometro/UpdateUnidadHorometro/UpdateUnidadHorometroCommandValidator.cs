using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.UpdateUnidadHorometro
{
    public class UpdateUnidadHorometroCommandValidator : AbstractValidator<UpdateUnidadHorometroCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUnidadHorometroCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.SubCentroCostoCode).NotEmpty().MaximumLength(6);
            RuleFor(x => x.Fecha).NotEmpty();

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

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
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
    }
}
