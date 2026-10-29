// Cepheus.Application/Features/Logistica/Transacciones/CotizacionProveedores/InvitarProveedor/InvitarProveedorCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionProveedores.InvitarProveedor
{
    public class InvitarProveedorCommandHandler : IRequestHandler<InvitarProveedorCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public InvitarProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(InvitarProveedorCommand request, CancellationToken cancellationToken)
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

            if (cotizacion.Estado is EstadoCotizacion.Cerrado or EstadoCotizacion.Anulado)
            {
                throw new InvalidOperationException($"La Cotización está en estado '{cotizacion.Estado}' y ya no admite invitar proveedores.");
            }

            if (cotizacion.Proveedores.Any(p => p.ProveedorCode == proveedorCode))
            {
                throw new InvalidOperationException($"El proveedor {proveedorCode} ya fue invitado a esta Cotización.");
            }

            var proveedor = new CotizacionProveedor
            {
                PlantaCode = plantaCode,
                CotizacionCode = cotizacionCode,
                ProveedorCode = proveedorCode,
                MonedaCode = request.MonedaCode.Trim().ToUpperInvariant(),
                Observaciones = request.Observaciones?.Trim() ?? string.Empty,
                Estado = EstadoProveedorCotizacion.Invitado
            };

            cotizacion.Proveedores.Add(proveedor);
            await _uow.Logistica.Transacciones.CotizacionProveedores.AddAsync(proveedor, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}