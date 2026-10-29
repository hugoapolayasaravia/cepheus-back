// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/UpdateImportacionDetalle/UpdateImportacionDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacionDetalle
{
    public class UpdateImportacionDetalleCommandHandler : IRequestHandler<UpdateImportacionDetalleCommand, ImportacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateImportacionDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(UpdateImportacionDetalleCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var importacionCode = request.ImportacionCode.Trim().ToUpperInvariant();
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var importacion = await _uow.Logistica.Transacciones.Importaciones.Query()
                .Include(i => i.Detalles)
                .Include(i => i.Gastos).ThenInclude(g => g.Articulos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(i => i.PlantaCode == plantaCode && i.Code == importacionCode, cancellationToken);

            if (importacion is null)
            {
                throw new KeyNotFoundException($"Importación {plantaCode}/{importacionCode} no encontrada.");
            }

            if (importacion.Estado != EstadoImportacion.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Importación está en estado '{importacion.Estado}' y ya no admite modificar artículos.");
            }

            var detalle = importacion.Detalles
                .FirstOrDefault(d => d.ProveedorCode == proveedorCode && d.ArticuloCode == articuloCode);

            if (detalle is null)
            {
                throw new KeyNotFoundException(
                    $"El artículo {articuloCode} del proveedor {proveedorCode} no existe en la Importación {plantaCode}/{importacionCode}.");
            }

            if (detalle.Estado != EstadoImportacion.Pendiente)
            {
                throw new InvalidOperationException("La línea ya fue procesada y no admite modificación.");
            }

            detalle.ComprobantePagoCode = request.ComprobantePagoCode.Trim().ToUpperInvariant();
            detalle.NumeroDocumento = request.NumeroDocumento.Trim().ToUpperInvariant();
            detalle.FechaEmision = request.FechaEmision;
            detalle.TipoCambio = await ImportacionTotalsCalculator.ResolveTipoCambioAsync(_uow, request.FechaEmision, cancellationToken);
            detalle.Cantidad = request.Cantidad;
            detalle.ValorFob = request.ValorFob;
            detalle.Flete = request.Flete;
            detalle.Seguro = request.Seguro;
            detalle.ValorAduana = request.ValorFob + request.Flete + request.Seguro;
            detalle.RowVersion = request.RowVersion;

            var igvPercentage = await ImportacionTotalsCalculator.GetIgvPercentageAsync(_uow, importacion.FechaPoliza, cancellationToken);
            ImportacionTotalsCalculator.Recalculate(importacion, igvPercentage);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("La Importación fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return ImportacionMapper.Map(importacion);
        }
    }
}
