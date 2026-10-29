using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Reportes.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesPreventivas.GenerateOrdenesPreventivas;

public sealed class GenerateOrdenesPreventivasCommandValidator : AbstractValidator<GenerateOrdenesPreventivasCommand>
{
    private readonly IUnitOfWork _uow;

    public GenerateOrdenesPreventivasCommandValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(x => x.PlantaCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("La planta es obligatoria.")
            .MaximumLength(2).WithMessage("El código de planta no puede exceder 2 caracteres.")
            .MustAsync(PlantaExists).WithMessage("La planta no existe.");

        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();

        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.")
            .OverridePropertyName(nameof(GenerateOrdenesPreventivasCommand.FechaDesde));

        RuleFor(x => x)
            .Must(x => (x.FechaHasta.Date - x.FechaDesde.Date).TotalDays < ReporteRules.MaxDiasCalendario)
            .WithMessage($"El rango no puede exceder {ReporteRules.MaxDiasCalendario} días.")
            .OverridePropertyName(nameof(GenerateOrdenesPreventivasCommand.FechaHasta))
            .When(x => x.FechaDesde.Date <= x.FechaHasta.Date);

        RuleForEach(x => x.PlanCodes)
            .NotEmpty().MaximumLength(6)
            .When(x => x.PlanCodes is not null);
    }

    private async Task<bool> PlantaExists(string code, CancellationToken ct)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == normalized, ct);
    }
}
