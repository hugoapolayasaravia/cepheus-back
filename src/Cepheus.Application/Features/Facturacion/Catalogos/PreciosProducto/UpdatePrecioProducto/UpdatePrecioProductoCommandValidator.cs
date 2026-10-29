using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.UpdatePrecioProducto
{
    public class UpdatePrecioProductoCommandValidator : AbstractValidator<UpdatePrecioProductoCommand>
    {
        public UpdatePrecioProductoCommandValidator()
        {
            RuleFor(x => x.FleteCode).NotEmpty().Length(2);
            RuleFor(x => x.ProductoTipoCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);
            RuleFor(x => x.CurrencyTypeCode).NotEmpty().Length(1);
            RuleFor(x => x.CurrencyCode).NotEmpty().Length(1);

            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TransportAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.FreightAmount).GreaterThanOrEqualTo(0);

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
