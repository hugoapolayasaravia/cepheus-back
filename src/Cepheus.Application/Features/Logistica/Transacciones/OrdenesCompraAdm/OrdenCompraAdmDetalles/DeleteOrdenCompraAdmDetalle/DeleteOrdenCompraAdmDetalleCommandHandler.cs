// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/DeleteOrdenCompraAdministracionDetalle/DeleteOrdenCompraAdministracionDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.DeleteOrdenCompraAdmDetalle
{
    public class DeleteOrdenCompraAdmDetalleCommandHandler
        : IRequestHandler<DeleteOrdenCompraAdmDetalleCommand, OrdenCompraAdmResponse>
    {
        private readonly IUnitOfWork _uow;

        public DeleteOrdenCompraAdmDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraAdmResponse> Handle(
            DeleteOrdenCompraAdmDetalleCommand request, CancellationToken cancellationToken)
        {
            var ordenCode = request.OrdenCompraAdministracionCode.Trim().ToUpperInvariant();

            var orden = await _uow.Logistica.Transacciones.OrdenesCompraAdministracion.Query()
                .Include(o => o.Proveedor)
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Code == ordenCode, cancellationToken);

            if (orden is null)
            {
                throw new KeyNotFoundException($"Orden de Compra de Administración {ordenCode} no encontrada.");
            }

            if (orden.Estado != EstadoOrdenCompraAdministracion.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Orden de Compra de Administración está en estado '{orden.Estado}' y ya no admite eliminar líneas.");
            }

            var detalle = orden.Detalles.FirstOrDefault(d => d.ItemNumber == request.ItemNumber);
            if (detalle is null)
            {
                throw new KeyNotFoundException(
                    $"La línea {request.ItemNumber} no existe en la Orden de Compra de Administración {ordenCode}.");
            }

            if (orden.Detalles.Count == 1)
            {
                throw new InvalidOperationException(
                    "La Orden de Compra de Administración debe conservar al menos una línea.");
            }

            orden.Detalles.Remove(detalle);
            _uow.Logistica.Transacciones.OrdenCompraAdministracionDetalles.Remove(detalle);

            await OrdenCompraAdmTotalsCalculator.RecalculateAsync(_uow, orden, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return OrdenCompraAdmMapper.Map(orden);
        }
    }
}
