using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.CreateHojaCorteDetalle;

public sealed class CreateHojaCorteDetalleCommandValidator : AbstractValidator<CreateHojaCorteDetalleCommand>
{
    public CreateHojaCorteDetalleCommandValidator()
    {
        RuleFor(x => x.Year).NotEmpty().Length(4);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull().SetValidator(new HojaCorteDetalleRequestValidator());
    }
}
