using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.UpdatePeriodoIgv;

public class UpdatePeriodoIgvCommandValidator : AbstractValidator<UpdatePeriodoIgvCommand>
{
    private readonly IUnitOfWork _uow;

    public UpdatePeriodoIgvCommandValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(x => x.FechaInicio)
            .NotEqual(default(DateOnly)).WithMessage("La fecha de inicio es obligatoria.");

        RuleFor(x => x.FechaFin)
            .Must((cmd, fin) => fin is null || fin.Value >= cmd.FechaInicio)
            .WithMessage("La fecha de fin no puede ser anterior a la de inicio.");

        RuleFor(x => x.PorcentajeIgv)
            .InclusiveBetween(0m, 100m).WithMessage("El porcentaje de IGV debe estar entre 0 y 100.");

        RuleFor(x => x)
            .MustAsync(NoSolaparConOtroPeriodo)
            .WithMessage("El período se solapa con otro período de IGV ya registrado.")
            .OverridePropertyName("FechaInicio");
    }

    private async Task<bool> NoSolaparConOtroPeriodo(UpdatePeriodoIgvCommand cmd, CancellationToken ct)
    {
        var otros = await _uow.Comunes.PeriodosIgv.Query().AsNoTracking()
            .Where(p => p.Id != cmd.Id)
            .Select(p => new { p.FechaInicio, p.FechaFin })
            .ToListAsync(ct);

        return !otros.Any(p => PeriodoIgvCalculo.Solapan(p.FechaInicio, p.FechaFin, cmd.FechaInicio, cmd.FechaFin));
    }
}
