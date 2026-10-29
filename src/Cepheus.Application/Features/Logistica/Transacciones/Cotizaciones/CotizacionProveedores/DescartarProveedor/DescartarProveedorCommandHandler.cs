// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/DescartarProveedor/DescartarProveedorCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.DescartarProveedor
{
    public class DescartarProveedorCommandHandler : IRequestHandler<DescartarProveedorCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public DescartarProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(DescartarProveedorCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var cotizacionCode = request.CotizacionCode.Trim().ToUpperInvariant();
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .Include(c => c.Proveedores)
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

            if (proveedor.Estado == EstadoProveedorCotizacion.Seleccionado)
            {
                throw new InvalidOperationException("No se puede descartar a un proveedor ya Seleccionado.");
            }

            proveedor.Estado = EstadoProveedorCotizacion.Descartado;

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}