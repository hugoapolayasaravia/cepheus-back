using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.UpdateUnidadParada
{
    public class UpdateUnidadParadaCommandValidator : AbstractValidator<UpdateUnidadParadaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUnidadParadaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.SubCentroCostoCode).NotEmpty().MaximumLength(6);
            RuleFor(x => x.Fecha).NotEmpty();

            RuleFor(x => x.Horas)
                .GreaterThan(0).WithMessage("Las horas de parada deben ser mayores a 0.")
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
            return await _uow.Mantenimiento.Catalogos.Fallas.Query().AnyAsync(f => f.Code == normalized, ct);
        }
    }
}
