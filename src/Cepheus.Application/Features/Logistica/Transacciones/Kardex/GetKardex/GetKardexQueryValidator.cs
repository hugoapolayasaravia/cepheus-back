using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Kardex.GetKardex;

public sealed class GetKardexQueryValidator : AbstractValidator<GetKardexQuery>
{
    public GetKardexQueryValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2)
            .WithMessage("La planta es obligatoria.");

        RuleFor(x => x.ArticuloCode).NotEmpty().MaximumLength(7)
            .WithMessage("El artículo es obligatorio.");

        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();

        RuleFor(x => x)
            .Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.");
    }
}
