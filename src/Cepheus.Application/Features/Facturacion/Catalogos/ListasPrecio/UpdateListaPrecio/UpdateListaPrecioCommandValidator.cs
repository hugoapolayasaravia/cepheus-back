using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.UpdateListaPrecio
{
    public class UpdateListaPrecioCommandValidator : AbstractValidator<UpdateListaPrecioCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateListaPrecioCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El identificador es obligatorio.");

            RuleFor(x => x)
                .MustAsync(ProductExists)
                .WithMessage("El producto indicado no existe.")
                .OverridePropertyName(nameof(UpdateListaPrecioCommand.ProductoCode));

            RuleFor(x => x.Precio)
                .GreaterThanOrEqualTo(0m).WithMessage("El precio no puede ser negativo.");

            RuleFor(x => x.FechaFin)
                .GreaterThan(x => x.FechaInicio).WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.")
                .When(x => x.FechaFin.HasValue);

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> ProductExists(UpdateListaPrecioCommand command, CancellationToken ct)
        {
            var tipo = command.TipoProductoCode.Trim().ToUpper();
            var code = command.ProductoCode.Trim().ToUpper();

            return await _uow.Facturacion.Maestros.Productos.Query().AnyAsync(p => p.TipoProductoCode == tipo && p.Code == code, ct);
        }
    }
}
