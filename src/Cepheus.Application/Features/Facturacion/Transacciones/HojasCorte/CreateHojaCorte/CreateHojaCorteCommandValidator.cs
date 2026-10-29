using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.CreateHojaCorte;

public sealed class CreateHojaCorteCommandValidator : HojaCorteHeaderValidator<CreateHojaCorteCommand>
{
    public CreateHojaCorteCommandValidator()
    {
        RuleFor(x => x.ParentYear).MaximumLength(4);
        RuleFor(x => x.ParentCode).MaximumLength(8);

        RuleForEach(x => x.Detalles).SetValidator(new HojaCorteDetalleRequestValidator());
    }
}
