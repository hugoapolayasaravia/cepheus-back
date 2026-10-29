using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.UnidadesDia.GetReporteUnidadesDia;

public sealed class GetReporteUnidadesDiaQueryValidator : AbstractValidator<GetReporteUnidadesDiaQuery>
{
    public GetReporteUnidadesDiaQueryValidator()
    {
        RuleFor(x => x.Fecha).NotEmpty().WithMessage("La fecha es obligatoria.");
    }
}
