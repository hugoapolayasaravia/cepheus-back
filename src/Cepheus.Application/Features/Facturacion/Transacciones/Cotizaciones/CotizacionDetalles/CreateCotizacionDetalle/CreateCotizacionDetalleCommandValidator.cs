using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.CreateCotizacionDetalle
{
    public class CreateCotizacionDetalleCommandValidator : AbstractValidator<CreateCotizacionDetalleCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionDetalleCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode).NotEmpty().Length(2);
            RuleFor(x => x.Year).NotEmpty().Length(4);
            RuleFor(x => x.Month).NotEmpty().Length(2);
            RuleFor(x => x.Code).NotEmpty().Length(8);

            RuleFor(x => x)
                .MustAsync(CotizacionEditable)
                .WithMessage("Solo se pueden agregar líneas a una cotización Pendiente y modificable.");

            RuleFor(x => x.ProductoTipoCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.ProductoCode).NotEmpty().MaximumLength(4);

            RuleFor(x => x)
                .MustAsync(ProductoExists)
                .WithMessage("El producto indicado no existe.");

            RuleFor(x => x.UnitCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La unidad de medida es obligatoria.")
                .MustAsync(UnitExists).WithMessage("La unidad de medida indicada no existe.");

            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.");
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Observations).NotNull().MaximumLength(50);
            RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        }

        private async Task<bool> CotizacionEditable(CreateCotizacionDetalleCommand c, CancellationToken ct)
        {
            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(x => x.NegocioCode == c.NegocioCode.Trim().ToUpper() && x.Year == c.Year
                                        && x.Month == c.Month && x.Code == c.Code.Trim().ToUpper(), ct);

            return cotizacion is not null && cotizacion.IsEditable && cotizacion.Status == EstadoCotizacion.Pendiente;
        }

        private async Task<bool> ProductoExists(CreateCotizacionDetalleCommand c, CancellationToken ct)
            => await _uow.Facturacion.Maestros.Productos.Query()
                .AnyAsync(p => p.TipoProductoCode == c.ProductoTipoCode.Trim().ToUpper()
                             && p.Code == c.ProductoCode.Trim().ToUpper(), ct);

        private async Task<bool> UnitExists(string code, CancellationToken ct)
            => await _uow.Facturacion.Catalogos.UnidadesMedidaVenta.Query()
                .AnyAsync(u => u.Code == code.Trim().ToUpper(), ct);
    }
}
