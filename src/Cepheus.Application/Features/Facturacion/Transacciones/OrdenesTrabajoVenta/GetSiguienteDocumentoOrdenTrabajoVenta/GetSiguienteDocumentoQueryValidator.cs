using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetSiguienteDocumentoOrdenTrabajoVenta;

public sealed class GetSiguienteDocumentoQueryValidator : AbstractValidator<GetSiguienteDocumentoQuery>
{
    public GetSiguienteDocumentoQueryValidator()
    {
        RuleFor(x => x.Planta).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Tipo).IsInEnum();
    }
}
