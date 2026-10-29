// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/UpdateOrdenCompraAdministracion/UpdateOrdenCompraAdministracionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.UpdateOrdenCompraAdm
{
    public class UpdateOrdenCompraAdmCommandHandler
        : IRequestHandler<UpdateOrdenCompraAdmCommand, OrdenCompraAdmResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOrdenCompraAdmCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraAdmResponse> Handle(
            UpdateOrdenCompraAdmCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim().ToUpperInvariant();

            var orden = await _uow.Logistica.Transacciones.OrdenesCompraAdministracion.Query()
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Code == code, cancellationToken);

            if (orden is null)
            {
                throw new KeyNotFoundException($"Orden de Compra de Administración {code} no encontrada.");
            }

            if (orden.Estado != EstadoOrdenCompraAdministracion.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Orden de Compra de Administración está en estado '{orden.Estado}' y ya no admite edición de cabecera.");
            }

            orden.ComprobantePagoCode = request.ComprobantePagoCode.Trim().ToUpperInvariant();
            orden.EmitirDocumento = request.EmitirDocumento?.Trim();
            orden.FechaEntrega = request.FechaEntrega;
            orden.ProveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();
            orden.CompradorCode = request.CompradorCode.Trim().ToUpperInvariant();
            orden.MonedaCode = request.MonedaCode.Trim().ToUpperInvariant();
            orden.LugarEnvioCode = request.LugarEnvioCode.Trim().ToUpperInvariant();
            orden.FormaPagoCode = request.FormaPagoCode.Trim().ToUpperInvariant();
            orden.TramiteCode = request.TramiteCode.Trim().ToUpperInvariant();
            orden.Observaciones1 = request.Observaciones1?.Trim();
            orden.Observaciones2 = request.Observaciones2?.Trim();
            orden.NotaCompraCode = string.IsNullOrWhiteSpace(request.NotaCompraCode)
                ? null
                : request.NotaCompraCode.Trim().ToUpperInvariant();
            orden.UnidadNegocioCode = request.UnidadNegocioCode.Trim().ToUpperInvariant();
            orden.Motivo = System.Enum.Parse<MotivoOrdenCompraAdministracion>(request.Motivo, true);
            orden.NoGravableCompra = request.NoGravableCompra;
            orden.ServicioCompra = request.ServicioCompra;
            orden.IgvExteriorCompra = request.IgvExteriorCompra;
            orden.RowVersion = request.RowVersion;

            // Recalcular totales por cambios que afectan la fórmula (comprobante, fecha, montos digitados).
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
