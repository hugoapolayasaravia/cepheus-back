// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/UpdateOrdenCompraAdministracionDetalle/UpdateOrdenCompraAdministracionDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.UpdateOrdenCompraAdmDetalle
{
    public class UpdateOrdenCompraAdmDetalleCommandHandler
        : IRequestHandler<UpdateOrdenCompraAdmDetalleCommand, OrdenCompraAdmResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOrdenCompraAdmDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraAdmResponse> Handle(
            UpdateOrdenCompraAdmDetalleCommand request, CancellationToken cancellationToken)
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
                    $"La Orden de Compra de Administración está en estado '{orden.Estado}' y ya no admite modificar líneas.");
            }

            var detalle = orden.Detalles.FirstOrDefault(d => d.ItemNumber == request.ItemNumber);
            if (detalle is null)
            {
                throw new KeyNotFoundException(
                    $"La línea {request.ItemNumber} no existe en la Orden de Compra de Administración {ordenCode}.");
            }

            detalle.Glosa = request.Glosa.Trim();
            detalle.UnidadMedidaCode = request.UnidadMedidaCode.Trim().ToUpperInvariant();
            detalle.Cantidad = request.Cantidad;
            detalle.Precio = request.Precio;
            detalle.Descuento = request.Descuento;
            detalle.Total = OrdenCompraAdmTotalsCalculator.CalculateLineTotal(
                request.Cantidad, request.Precio, request.Descuento);
            detalle.SubCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpperInvariant();

            await OrdenCompraAdmTotalsCalculator.RecalculateAsync(_uow, orden, cancellationToken);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La Orden de Compra de Administración fue modificada por otro proceso. " +
                    "Recargue los datos e intente nuevamente.");
            }

            return OrdenCompraAdmMapper.Map(orden);
        }
    }
}
