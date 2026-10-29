// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/RecalcularProrrateoImportacion/RecalcularProrrateoImportacionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.RecalcularProrrateoImportacion
{
    public class RecalcularProrrateoImportacionCommandHandler : IRequestHandler<RecalcularProrrateoImportacionCommand, ImportacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public RecalcularProrrateoImportacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(RecalcularProrrateoImportacionCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var importacionCode = request.ImportacionCode.Trim().ToUpperInvariant();

            var importacion = await _uow.Logistica.Transacciones.Importaciones.Query()
                .Include(i => i.Detalles).ThenInclude(d => d.Articulo)
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
                    $"La Importación está en estado '{importacion.Estado}' y ya no admite recalcular el prorrateo.");
            }

            var igvPercentage = await ImportacionTotalsCalculator.GetIgvPercentageAsync(_uow, importacion.FechaPoliza, cancellationToken);

            // Totales de cabecera + prorrateo a líneas y gastos.
            ImportacionTotalsCalculator.Recalculate(importacion, igvPercentage);
            ImportacionProrrateoCalculator.Calculate(importacion, igvPercentage);

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
