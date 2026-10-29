// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/DeleteImportacionGasto/DeleteImportacionGastoCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.DeleteImportacionGasto
{
    public class DeleteImportacionGastoCommandHandler : IRequestHandler<DeleteImportacionGastoCommand, ImportacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public DeleteImportacionGastoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(DeleteImportacionGastoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var importacionCode = request.ImportacionCode.Trim().ToUpperInvariant();
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();
            var numeroDocumento = request.NumeroDocumento.Trim().ToUpperInvariant();

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
                    $"La Importación está en estado '{importacion.Estado}' y ya no admite eliminar gastos.");
            }

            var gasto = importacion.Gastos
                .FirstOrDefault(g => g.ProveedorCode == proveedorCode && g.NumeroDocumento == numeroDocumento)
                ?? throw new KeyNotFoundException(
                    $"El documento {numeroDocumento} del proveedor {proveedorCode} no existe en la Importación {plantaCode}/{importacionCode}.");

            // El prorrateo por artículo del gasto se elimina en cascada.
            importacion.Gastos.Remove(gasto);
            _uow.Logistica.Transacciones.ImportacionGastos.Remove(gasto);

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
