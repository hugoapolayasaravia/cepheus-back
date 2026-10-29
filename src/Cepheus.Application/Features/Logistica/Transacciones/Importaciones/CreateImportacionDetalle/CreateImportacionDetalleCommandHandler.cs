// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/CreateImportacionDetalle/CreateImportacionDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacionDetalle
{
    public class CreateImportacionDetalleCommandHandler : IRequestHandler<CreateImportacionDetalleCommand, ImportacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateImportacionDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(CreateImportacionDetalleCommand request, CancellationToken cancellationToken)
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

            // Legacy (ue_ins_lin_det): solo se agregan líneas en estado Pendiente.
            if (importacion.Estado != EstadoImportacion.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Importación está en estado '{importacion.Estado}' y ya no admite agregar artículos.");
            }

            // Legacy (fg_buscacodigo IMPORTACIONES_ARTICULOS): clave Proveedor + Artículo única.
            if (importacion.Detalles.Any(d => d.ProveedorCode == proveedorCode && d.ArticuloCode == articuloCode))
            {
                throw new InvalidOperationException(
                    $"El artículo {articuloCode} ya existe en la Importación para el proveedor {proveedorCode}.");
            }

            var detalle = new ImportacionDetalle
            {
                PlantaCode = plantaCode,
                ImportacionCode = importacionCode,
                ProveedorCode = proveedorCode,
                ArticuloCode = articuloCode,
                ComprobantePagoCode = request.ComprobantePagoCode.Trim().ToUpperInvariant(),
                NumeroDocumento = request.NumeroDocumento.Trim().ToUpperInvariant(),
                FechaEmision = request.FechaEmision,
                TipoCambio = await ImportacionTotalsCalculator.ResolveTipoCambioAsync(_uow, request.FechaEmision, cancellationToken),
                Cantidad = request.Cantidad,
                ValorFob = request.ValorFob,
                Flete = request.Flete,
                Seguro = request.Seguro,
                ValorAduana = request.ValorFob + request.Flete + request.Seguro,
                Estado = EstadoImportacion.Pendiente
            };

            importacion.Detalles.Add(detalle);

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
