using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.UpdateHojaCorte;

public sealed class UpdateHojaCorteCommandValidator : AbstractValidator<UpdateHojaCorteCommand>
{
    private sealed class DataValidator : HojaCorteHeaderValidator<UpdateHojaCorteRequest>
    {
        public DataValidator()
        {
            RuleFor(x => x.Date).NotEmpty().WithMessage("La fecha es obligatoria.");
        }
    }

    public UpdateHojaCorteCommandValidator()
    {
        RuleFor(x => x.Year).NotEmpty().Length(4);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull().SetValidator(new DataValidator());
    }
}
