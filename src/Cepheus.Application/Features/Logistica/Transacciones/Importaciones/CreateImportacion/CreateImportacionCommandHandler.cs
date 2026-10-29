// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/CreateImportacion/CreateImportacionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacion
{
    public class CreateImportacionCommandHandler : IRequestHandler<CreateImportacionCommand, ImportacionResponse>
    {
        private const int MaxConcurrencyRetries = 3;
        private const int CorrelativeLength = 6;

        private readonly IUnitOfWork _uow;

        public CreateImportacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(CreateImportacionCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            var tipoCambio = await ImportacionTotalsCalculator.ResolveTipoCambioAsync(_uow, request.FechaPoliza, cancellationToken);
            var igvPercentage = await ImportacionTotalsCalculator.GetIgvPercentageAsync(_uow, request.FechaPoliza, cancellationToken);

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(plantaCode, cancellationToken);

                var importacion = new Importacion
                {
                    PlantaCode = plantaCode,
                    Code = nextCode,
                    PesoNeto = request.PesoNeto,
                    PesoBruto = request.PesoBruto,
                    FechaPoliza = request.FechaPoliza,
                    FechaEntrega = request.FechaEntrega,
                    TipoCambio = tipoCambio,
                    Advalorem = request.Advalorem,
                    Sobretasa = request.Sobretasa,
                    OtrosGastos = request.OtrosGastos,
                    Estado = EstadoImportacion.Pendiente
                };

                // Sin detalle aún: los totales de Fob/Flete/Seguro parten en 0.
                ImportacionTotalsCalculator.Recalculate(importacion, 0, 0, 0, igvPercentage);

                await _uow.Logistica.Transacciones.Importaciones.AddAsync(importacion, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return ImportacionMapper.Map(importacion);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                    // Colisión de correlativo por creación simultánea en la misma planta.
                    _uow.Logistica.Transacciones.Importaciones.Remove(importacion);
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo de Importación para la planta {plantaCode} por alta concurrencia. Intente nuevamente.");
        }

        private async Task<string> NextCodeAsync(string plantaCode, CancellationToken cancellationToken)
        {
            var lastCode = await _uow.Logistica.Transacciones.Importaciones.Query()
                .Where(i => i.PlantaCode == plantaCode)
                .OrderByDescending(i => i.Code)
                .Select(i => i.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (lastCode is not null && int.TryParse(lastCode, out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return next.ToString().PadLeft(CorrelativeLength, '0');
        }
    }
}
