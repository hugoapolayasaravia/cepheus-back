// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/RegistrarRespuestaProveedor/RegistrarRespuestaProveedorCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.RegistrarRespuestaProveedor
{
    public class RegistrarRespuestaProveedorCommandHandler : IRequestHandler<RegistrarRespuestaProveedorCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public RegistrarRespuestaProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(RegistrarRespuestaProveedorCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var cotizacionCode = request.CotizacionCode.Trim().ToUpperInvariant();
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .Include(c => c.Proveedores).ThenInclude(p => p.Detalles)
                .FirstOrDefaultAsync(c => c.PlantaCode == plantaCode && c.Code == cotizacionCode, cancellationToken);

            if (cotizacion is null)
            {
                throw new KeyNotFoundException($"Cotización {plantaCode}/{cotizacionCode} no encontrada.");
            }

            var proveedor = cotizacion.Proveedores.FirstOrDefault(p => p.ProveedorCode == proveedorCode);
            if (proveedor is null)
            {
                throw new KeyNotFoundException($"El proveedor {proveedorCode} no fue invitado a esta Cotización.");
            }

            if (proveedor.Estado is EstadoProveedorCotizacion.Seleccionado or EstadoProveedorCotizacion.Descartado)
            {
                throw new InvalidOperationException($"El proveedor ya está en estado '{proveedor.Estado}' y no admite registrar una nueva respuesta.");
            }

            // Reemplazo completo de las líneas — mismo criterio que "reemplazar oferta".
            foreach (var linea in proveedor.Detalles.ToList())
            {
                _uow.Logistica.Transacciones.CotizacionProveedorDetalles.Remove(linea);
            }
            proveedor.Detalles.Clear();

            foreach (var linea in request.Lineas)
            {
                var articuloCode = linea.ArticuloCode.Trim().ToUpperInvariant();
                var nuevaLinea = new CotizacionProveedorDetalle
                {
                    PlantaCode = plantaCode,
                    CotizacionCode = cotizacionCode,
                    ProveedorCode = proveedorCode,
                    ArticuloCode = articuloCode,
                    CantidadArticulo = linea.CantidadArticulo,
                    PrecioArticulo = linea.PrecioArticulo,
                    DescuentoArticulo = linea.DescuentoArticulo,
                    TotalLinea = CotizacionCalculators.CalculateLineTotal(linea.CantidadArticulo, linea.PrecioArticulo, linea.DescuentoArticulo)
                };
                proveedor.Detalles.Add(nuevaLinea);
                await _uow.Logistica.Transacciones.CotizacionProveedorDetalles.AddAsync(nuevaLinea, cancellationToken);
            }

            proveedor.Estado = EstadoProveedorCotizacion.Respondido;
            proveedor.FechaRespuesta = DateTime.UtcNow;

            await CotizacionCalculators.RecalculateProveedorAsync(_uow, proveedor, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}