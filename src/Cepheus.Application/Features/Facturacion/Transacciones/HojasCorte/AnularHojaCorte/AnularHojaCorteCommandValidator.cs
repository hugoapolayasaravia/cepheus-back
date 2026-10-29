using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.AnularHojaCorte;

public sealed class AnularHojaCorteCommandValidator : AbstractValidator<AnularHojaCorteCommand>
{
    public AnularHojaCorteCommandValidator()
    {
        RuleFor(x => x.Year).NotEmpty().Length(4);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
    }
}
