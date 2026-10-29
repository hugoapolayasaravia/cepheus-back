// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion.OrdenCompraAdministracionDetalles/CreateOrdenCompraAdministracionDetalle/CreateOrdenCompraAdministracionDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.OrdenCompraAdmDetalles.CreateOrdenCompraAdmDetalle
{
    public class CreateOrdenCompraAdmDetalleCommandHandler
        : IRequestHandler<CreateOrdenCompraAdmDetalleCommand, OrdenCompraAdmResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateOrdenCompraAdmDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraAdmResponse> Handle(
            CreateOrdenCompraAdmDetalleCommand request, CancellationToken cancellationToken)
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
                    $"La Orden de Compra de Administración está en estado '{orden.Estado}' y ya no admite agregar líneas.");
            }

            var nextItem = orden.Detalles.Count == 0 ? 1 : orden.Detalles.Max(d => d.ItemNumber) + 1;

            var detalle = new OrdenCompraAdministracionDetalle
            {
                OrdenCompraAdministracionCode = ordenCode,
                ItemNumber = nextItem,
                Glosa = request.Glosa.Trim(),
                UnidadMedidaCode = request.UnidadMedidaCode.Trim().ToUpperInvariant(),
                Cantidad = request.Cantidad,
                Precio = request.Precio,
                Descuento = request.Descuento,
                Total = OrdenCompraAdmTotalsCalculator.CalculateLineTotal(
                    request.Cantidad, request.Precio, request.Descuento),
                SubCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpperInvariant(),
                CantidadEntregada = 0
            };

            orden.Detalles.Add(detalle);
            await _uow.Logistica.Transacciones.OrdenCompraAdministracionDetalles.AddAsync(detalle, cancellationToken);

            await OrdenCompraAdmTotalsCalculator.RecalculateAsync(_uow, orden, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return OrdenCompraAdmMapper.Map(orden);
        }
    }
}
