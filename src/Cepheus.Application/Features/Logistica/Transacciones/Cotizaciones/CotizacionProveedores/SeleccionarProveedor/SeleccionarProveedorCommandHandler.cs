// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/SeleccionarProveedor/SeleccionarProveedorCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.SeleccionarProveedor
{
    /// <summary>
    /// Selección por proveedor completo (confirmado). Decisión propia (a
    /// confirmar si no aplica): al seleccionar un proveedor, los demás que
    /// estén en Respondido pasan automáticamente a Descartado — asume
    /// adjudicación única por Cotización. Los que siguen en Invitado (nunca
    /// respondieron) se dejan tal cual, no se fuerzan a Descartado.
    /// </summary>
    public class SeleccionarProveedorCommandHandler : IRequestHandler<SeleccionarProveedorCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public SeleccionarProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(SeleccionarProveedorCommand request, CancellationToken cancellationToken)
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

            if (proveedor.Estado != EstadoProveedorCotizacion.Respondido)
            {
                throw new InvalidOperationException(
                    $"Solo se puede seleccionar un proveedor en estado 'Respondido' (actual: '{proveedor.Estado}').");
            }

            proveedor.Estado = EstadoProveedorCotizacion.Seleccionado;

            foreach (var otro in cotizacion.Proveedores.Where(p => p.ProveedorCode != proveedorCode
                                                                  && p.Estado == EstadoProveedorCotizacion.Respondido))
            {
                otro.Estado = EstadoProveedorCotizacion.Descartado;
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}