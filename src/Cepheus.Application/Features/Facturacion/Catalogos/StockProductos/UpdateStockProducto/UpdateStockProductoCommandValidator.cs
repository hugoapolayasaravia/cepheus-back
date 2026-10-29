using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.UpdateStockProducto
{
    public class UpdateStockProductoCommandValidator : AbstractValidator<UpdateStockProductoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateStockProductoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El identificador es obligatorio.");

            RuleFor(x => x.PlantaCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La planta es obligatoria.")
                .MustAsync(PlantaExists).WithMessage("La planta indicada no existe.");

            RuleFor(x => x)
                .MustAsync(ProductExists)
                .WithMessage("El producto indicado no existe.")
                .OverridePropertyName(nameof(UpdateStockProductoCommand.ProductoCode));

            RuleFor(x => x.Cantidad)
                .GreaterThanOrEqualTo(0m).WithMessage("La cantidad no puede ser negativa.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> PlantaExists(string code, CancellationToken ct)
            => await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == code.Trim().ToUpper(), ct);

        private async Task<bool> ProductExists(UpdateStockProductoCommand command, CancellationToken ct)
        {
            var tipo = command.TipoProductoCode.Trim().ToUpper();
            var code = command.ProductoCode.Trim().ToUpper();

            return await _uow.Facturacion.Maestros.Productos.Query().AnyAsync(p => p.TipoProductoCode == tipo && p.Code == code, ct);
        }
    }
}
