using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.UpdateCotizacionDetalle
{
    public class UpdateCotizacionDetalleCommandValidator : AbstractValidator<UpdateCotizacionDetalleCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCotizacionDetalleCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.NegocioCode).NotEmpty().Length(2);
            RuleFor(x => x.Year).NotEmpty().Length(4);
            RuleFor(x => x.Month).NotEmpty().Length(2);
            RuleFor(x => x.Code).NotEmpty().Length(8);
            RuleFor(x => x.Item).GreaterThan(0);

            RuleFor(x => x)
                .MustAsync(CotizacionEditable)
                .WithMessage("Solo se pueden editar líneas de una cotización Pendiente y modificable.");

            RuleFor(x => x.UnitCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (code, ct) => await _uow.Facturacion.Catalogos.UnidadesMedidaVenta.Query()
                    .AnyAsync(u => u.Code == code.Trim().ToUpper(), ct))
                .WithMessage("La unidad de medida indicada no existe.");

            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Observations).NotNull().MaximumLength(50);
            RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }

        private async Task<bool> CotizacionEditable(UpdateCotizacionDetalleCommand c, CancellationToken ct)
        {
            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(x => x.NegocioCode == c.NegocioCode.Trim().ToUpper() && x.Year == c.Year
                                        && x.Month == c.Month && x.Code == c.Code.Trim().ToUpper(), ct);

            return cotizacion is not null && cotizacion.IsEditable && cotizacion.Status == EstadoCotizacion.Pendiente;
        }
    }
}
