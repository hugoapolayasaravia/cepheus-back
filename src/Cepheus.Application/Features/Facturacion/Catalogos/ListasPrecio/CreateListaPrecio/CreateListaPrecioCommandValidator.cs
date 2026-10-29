using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.CreateListaPrecio
{
    public class CreateListaPrecioCommandValidator : AbstractValidator<CreateListaPrecioCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateListaPrecioCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x)
                .MustAsync(ProductExists)
                .WithMessage("El producto indicado no existe.")
                .OverridePropertyName(nameof(CreateListaPrecioCommand.ProductoCode));

            RuleFor(x => x.Precio)
                .GreaterThanOrEqualTo(0m).WithMessage("El precio no puede ser negativo.");

            RuleFor(x => x.FechaFin)
                .GreaterThan(x => x.FechaInicio).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.")
                .When(x => x.FechaFin.HasValue);

        }

        private async Task<bool> ProductExists(CreateListaPrecioCommand command, CancellationToken ct)
        {
            var tipo = command.TipoProductoCode.Trim().ToUpper();
            var code = command.ProductoCode.Trim().ToUpper();

            return await _uow.Facturacion.Maestros.Productos.Query().AnyAsync(p => p.TipoProductoCode == tipo && p.Code == code, ct);
        }
    }
}
