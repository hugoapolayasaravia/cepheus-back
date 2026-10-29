using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common
{
    /// <summary>Reglas de una línea de nota de débito (se usan al crear y al modificar).</summary>
    public class NotaDebitoVentaDetalleInputValidator : AbstractValidator<NotaDebitoVentaDetalleInput>
    {
        public NotaDebitoVentaDetalleInputValidator()
        {
            RuleFor(l => l.TipoProductoCode)
                .NotEmpty().WithMessage("El tipo de producto de la línea es obligatorio.")
                .MaximumLength(2).WithMessage("El tipo de producto debe tener 2 caracteres.");

            RuleFor(l => l.ProductoCode)
                .NotEmpty().WithMessage("El producto de la línea es obligatorio.")
                .MaximumLength(4).WithMessage("El código de producto debe tener 4 caracteres.");

            RuleFor(l => l.Description)
                .MaximumLength(255).WithMessage("La descripción de la línea no puede exceder los 255 caracteres.");

            RuleFor(l => l.Quantity)
                .GreaterThan(0).WithMessage("La cantidad de la línea debe ser mayor a cero.");

            RuleFor(l => l.UnitPrice)
                .GreaterThan(0).WithMessage("El precio de la línea debe ser mayor a cero.");

            RuleFor(l => l.LineKey)
                .GreaterThan(0).WithMessage("La clave de línea debe ser mayor a cero.")
                .When(l => l.LineKey.HasValue);
        }
    }

    public static class NotaDebitoVentaDetailsValidation
    {
        public static bool NoDuplicateLines(List<NotaDebitoVentaDetalleInput> lines)
        {
            var keys = lines
                .Select((l, i) => $"{l.TipoProductoCode?.Trim()}|{l.ProductoCode?.Trim()}|{l.LineKey ?? (i + 1)}")
                .ToList();

            return keys.Distinct().Count() == keys.Count;
        }

        public static async Task<bool> ProductsExistAsync(
            IUnitOfWork uow, List<NotaDebitoVentaDetalleInput> lines, CancellationToken cancellationToken)
        {
            var pairs = lines
                .Where(l => !string.IsNullOrWhiteSpace(l.TipoProductoCode) && !string.IsNullOrWhiteSpace(l.ProductoCode))
                .Select(l => (Tipo: l.TipoProductoCode.Trim(), Producto: l.ProductoCode.Trim()))
                .Distinct()
                .ToList();

            foreach (var (tipo, producto) in pairs)
            {
                var exists = await uow.Facturacion.Maestros.Productos.Query()
                    .AnyAsync(p => p.TipoProductoCode == tipo && p.Code == producto, cancellationToken);

                if (!exists)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
