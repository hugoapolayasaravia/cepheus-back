using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.CopiarHojaCorte;

public sealed class CopiarHojaCorteCommandValidator : AbstractValidator<CopiarHojaCorteCommand>
{
    public CopiarHojaCorteCommandValidator()
    {
        RuleFor(x => x.Year).NotEmpty().Length(4);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
    }
}
