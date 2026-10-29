// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/UpdateOrdenCompra/UpdateOrdenCompraCommandHandler.cs

using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.UpdateOrdenCompra
{
    public class UpdateOrdenCompraCommandHandler
        : IRequestHandler<UpdateOrdenCompraCommand, OrdenCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOrdenCompraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraResponse> Handle(
            UpdateOrdenCompraCommand request,
            CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var orden = await _uow.Logistica.Transacciones.OrdenesCompra.Query()
                .Include(o => o.Detalles)
                .ThenInclude(d => d.Origenes)
                .FirstOrDefaultAsync(
                    o => o.PlantaCode == plantaCode && o.Code == code,
                    cancellationToken);

            if (orden is null)
            {
                throw new KeyNotFoundException(
                    $"Orden de Compra {plantaCode}/{code} no encontrada.");
            }

            if (orden.Estado != EstadoOrdenCompra.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Orden de Compra está en estado '{orden.Estado}' y ya no admite edición de cabecera.");
            }

            orden.TipoCompraCode = request.TipoCompraCode.Trim().ToUpperInvariant();
            orden.ComprobantePagoCode = request.ComprobantePagoCode;
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
            orden.EnviarCorreoProveedor = request.EnviarCorreoProveedor;
            orden.MotivoRetraso = request.MotivoRetraso?.Trim();
            orden.NoGravableCompra = request.NoGravableCompra;
            orden.ServicioCompra = request.ServicioCompra;
            orden.IgvExteriorCompra = request.IgvExteriorCompra;
            orden.RowVersion = request.RowVersion;

            // Recalcular totales por cambios que afectan la fórmula.
            await OrdenCompraTotalsCalculator.RecalculateAsync(
                _uow,
                orden,
                cancellationToken);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La Orden de Compra fue modificada por otro proceso. " +
                    "Recargue los datos e intente nuevamente.");
            }

            return OrdenCompraMapper.Map(orden);
        }
    }
}