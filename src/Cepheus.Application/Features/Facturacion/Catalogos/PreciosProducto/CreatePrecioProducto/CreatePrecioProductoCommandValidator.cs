using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.CreatePrecioProducto
{
    public class CreatePrecioProductoCommandValidator : AbstractValidator<CreatePrecioProductoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreatePrecioProductoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.FleteCode).NotEmpty().Length(2);
            RuleFor(x => x.ProductoTipoCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);
            RuleFor(x => x.CurrencyTypeCode).NotEmpty().Length(1);
            RuleFor(x => x.CurrencyCode).NotEmpty().Length(1);

            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.TransportAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.FreightAmount).GreaterThanOrEqualTo(0);

            RuleFor(x => x)
                .MustAsync(NotExist)
                .WithMessage("Ya existe un precio registrado para esa combinación de flete, producto y moneda.");
        }

        private async Task<bool> NotExist(CreatePrecioProductoCommand c, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.PreciosProducto.Query()
                .AnyAsync(p =>
                    p.FleteCode == c.FleteCode &&
                    p.ProductoTipoCode == c.ProductoTipoCode &&
                    p.ProductoCode == c.ProductoCode &&
                    p.CurrencyTypeCode == c.CurrencyTypeCode &&
                    p.CurrencyCode == c.CurrencyCode, cancellationToken);
    }
}
