// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/CreateImportacionGasto/CreateImportacionGastoCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacionGasto
{
    public class CreateImportacionGastoCommandHandler : IRequestHandler<CreateImportacionGastoCommand, ImportacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateImportacionGastoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(CreateImportacionGastoCommand request, CancellationToken cancellationToken)
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
                    $"La Importación está en estado '{importacion.Estado}' y ya no admite agregar gastos.");
            }

            if (importacion.Gastos.Any(g => g.ProveedorCode == proveedorCode && g.NumeroDocumento == numeroDocumento))
            {
                throw new InvalidOperationException(
                    $"El documento {numeroDocumento} del proveedor {proveedorCode} ya existe como gasto de la Importación.");
            }

            var comprobante = await _uow.Comunes.ComprobantesPago.Query()
                .AsNoTracking()
                .FirstAsync(c => c.Code == request.ComprobantePagoCode.Trim().ToUpper(), cancellationToken);

            var gasto = new ImportacionGasto
            {
                PlantaCode = plantaCode,
                ImportacionCode = importacionCode,
                ProveedorCode = proveedorCode,
                NumeroDocumento = numeroDocumento,
                ComprobantePagoCode = request.ComprobantePagoCode.Trim().ToUpperInvariant(),
                MonedaCode = request.MonedaCode.Trim().ToUpperInvariant(),
                Afecto = request.Afecto,
                FechaEmision = request.FechaEmision,
                TipoCambio = await ImportacionTotalsCalculator.ResolveTipoCambioAsync(_uow, request.FechaEmision, cancellationToken),
                NetoGasto = request.NetoGasto,
                NetoGastoInafecto = request.NetoGastoInafecto
            };

            var igvPercentage = await ImportacionTotalsCalculator.GetIgvPercentageAsync(_uow, request.FechaEmision, cancellationToken);
            ImportacionGastoCalculator.Apply(gasto, comprobante, igvPercentage, request.Igv);

            importacion.Gastos.Add(gasto);

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
