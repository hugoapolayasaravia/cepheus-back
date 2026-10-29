using Cepheus.Domain.Facturacion.Transacciones;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.HojaCorteDetalles.DeleteHojaCorteDetalle;

public sealed class DeleteHojaCorteDetalleCommandValidator : AbstractValidator<DeleteHojaCorteDetalleCommand>
{
    public DeleteHojaCorteDetalleCommandValidator()
    {
        RuleFor(x => x.Year).NotEmpty().Length(4);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.ProductoTipoCode).NotEmpty()
            .Must(t => HojaCorteProductoTipos.IsValid(t?.Trim() ?? string.Empty))
            .WithMessage("ProductoTipoCode debe ser 16 (vigueta) o 18 (bovedilla).");
        RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);
        RuleFor(x => x.ItemNumber).GreaterThan(0);
    }
}
