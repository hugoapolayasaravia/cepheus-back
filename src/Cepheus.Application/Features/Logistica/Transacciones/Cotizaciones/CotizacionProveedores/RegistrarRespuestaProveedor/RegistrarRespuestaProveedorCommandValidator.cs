// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/RegistrarRespuestaProveedor/RegistrarRespuestaProveedorCommandValidator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.RegistrarRespuestaProveedor
{
    public class RegistrarRespuestaProveedorCommandValidator : AbstractValidator<RegistrarRespuestaProveedorCommand>
    {
        private readonly IUnitOfWork _uow;

        public RegistrarRespuestaProveedorCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.CotizacionCode).NotEmpty();
            RuleFor(x => x.ProveedorCode).NotEmpty();
            RuleFor(x => x.Lineas).NotEmpty().WithMessage("La respuesta debe traer al menos una línea de precio.");

            RuleForEach(x => x.Lineas).ChildRules(l =>
            {
                l.RuleFor(x => x.CantidadArticulo).GreaterThan(0);
                l.RuleFor(x => x.PrecioArticulo).GreaterThanOrEqualTo(0);
                l.RuleFor(x => x.DescuentoArticulo).GreaterThanOrEqualTo(0);
            });

            RuleFor(x => x)
                .MustAsync(LineasCorrespondenAlDetalleDeLaCotizacion)
                .WithMessage("Todas las líneas de precio deben corresponder a artículos incluidos en la Cotización.");
        }

        private async Task<bool> LineasCorrespondenAlDetalleDeLaCotizacion(RegistrarRespuestaProveedorCommand cmd, CancellationToken ct)
        {
            var plantaCode = cmd.PlantaCode.Trim().ToUpperInvariant();
            var cotizacionCode = cmd.CotizacionCode.Trim().ToUpperInvariant();

            var articulosCotizacion = await _uow.Logistica.Transacciones.CotizacionDetalles.Query()
                .Where(d => d.PlantaCode == plantaCode && d.CotizacionCode == cotizacionCode)
                .Select(d => d.ArticuloCode)
                .ToListAsync(ct);

            return cmd.Lineas.All(l => articulosCotizacion.Contains(l.ArticuloCode.Trim().ToUpperInvariant()));
        }
    }
}