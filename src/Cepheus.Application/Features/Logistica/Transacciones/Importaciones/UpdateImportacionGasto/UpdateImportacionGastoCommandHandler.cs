// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/UpdateImportacionGasto/UpdateImportacionGastoCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacionGasto
{
    public class UpdateImportacionGastoCommandHandler : IRequestHandler<UpdateImportacionGastoCommand, ImportacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateImportacionGastoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(UpdateImportacionGastoCommand request, CancellationToken cancellationToken)
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
                    $"La Importación está en estado '{importacion.Estado}' y ya no admite modificar gastos.");
            }

            var gasto = importacion.Gastos
                .FirstOrDefault(g => g.ProveedorCode == proveedorCode && g.NumeroDocumento == numeroDocumento)
                ?? throw new KeyNotFoundException(
                    $"El documento {numeroDocumento} del proveedor {proveedorCode} no existe en la Importación {plantaCode}/{importacionCode}.");

            var comprobante = await _uow.Comunes.ComprobantesPago.Query()
                .AsNoTracking()
                .FirstAsync(c => c.Code == request.ComprobantePagoCode.Trim().ToUpper(), cancellationToken);

            gasto.ComprobantePagoCode = request.ComprobantePagoCode.Trim().ToUpperInvariant();
            gasto.MonedaCode = request.MonedaCode.Trim().ToUpperInvariant();
            gasto.Afecto = request.Afecto;
            gasto.FechaEmision = request.FechaEmision;
            gasto.TipoCambio = await ImportacionTotalsCalculator.ResolveTipoCambioAsync(_uow, request.FechaEmision, cancellationToken);
            gasto.NetoGasto = request.NetoGasto;
            gasto.NetoGastoInafecto = request.NetoGastoInafecto;
            gasto.RowVersion = request.RowVersion;

            var igvPercentage = await ImportacionTotalsCalculator.GetIgvPercentageAsync(_uow, request.FechaEmision, cancellationToken);
            ImportacionGastoCalculator.Apply(gasto, comprobante, igvPercentage, request.Igv);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("El gasto fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return ImportacionMapper.Map(importacion);
        }
    }
}
