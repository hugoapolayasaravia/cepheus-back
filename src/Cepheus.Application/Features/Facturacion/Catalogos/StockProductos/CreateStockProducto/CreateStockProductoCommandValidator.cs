using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.CreateStockProducto
{
    public class CreateStockProductoCommandValidator : AbstractValidator<CreateStockProductoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateStockProductoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x)
                .MustAsync(ProductExists)
                .WithMessage("El producto indicado no existe.")
                .OverridePropertyName(nameof(CreateStockProductoCommand.ProductoCode));

            RuleFor(x => x)
                .MustAsync(BeUniqueStock)
                .WithMessage("Ya existe un registro de stock para ese producto en esa planta.")
                .OverridePropertyName(nameof(CreateStockProductoCommand.ProductoCode));

            RuleFor(x => x.Cantidad)
                .GreaterThanOrEqualTo(0m).WithMessage("La cantidad no puede ser negativa.");

        }

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ProductExists(CreateStockProductoCommand command, CancellationToken ct)
        {
            var tipo = command.TipoProductoCode.Trim().ToUpper();
            var code = command.ProductoCode.Trim().ToUpper();

            return await _uow.Facturacion.Maestros.Productos.Query().AnyAsync(p => p.TipoProductoCode == tipo && p.Code == code, ct);
        }

        private async Task<bool> BeUniqueStock(CreateStockProductoCommand command, CancellationToken ct)
        {
            var planta = command.PlantaCode.Trim().ToUpper();
            var tipo = command.TipoProductoCode.Trim().ToUpper();
            var code = command.ProductoCode.Trim().ToUpper();

            return !await _uow.Facturacion.Catalogos.StockProductos.Query()
                .AnyAsync(x => x.PlantaCode == planta && x.TipoProductoCode == tipo && x.ProductoCode == code, ct);
        }
    }
}
