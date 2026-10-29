// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/UpdateImportacion/UpdateImportacionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacion
{
    public class UpdateImportacionCommandHandler : IRequestHandler<UpdateImportacionCommand, ImportacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateImportacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(UpdateImportacionCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var importacion = await _uow.Logistica.Transacciones.Importaciones.Query()
                .Include(i => i.Detalles)
                .Include(i => i.Gastos).ThenInclude(g => g.Articulos)
                .AsSplitQuery()
                .FirstOrDefaultAsync(i => i.PlantaCode == plantaCode && i.Code == code, cancellationToken);

            if (importacion is null)
            {
                throw new KeyNotFoundException($"Importación {plantaCode}/{code} no encontrada.");
            }

            // Legacy (ue_set_btn_mod_dw): solo se modifica en estado Pendiente.
            if (importacion.Estado != EstadoImportacion.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Importación está en estado '{importacion.Estado}' y ya no admite modificación.");
            }

            importacion.PesoNeto = request.PesoNeto;
            importacion.PesoBruto = request.PesoBruto;
            importacion.FechaPoliza = request.FechaPoliza;
            importacion.FechaEntrega = request.FechaEntrega;
            importacion.Advalorem = request.Advalorem;
            importacion.Sobretasa = request.Sobretasa;
            importacion.OtrosGastos = request.OtrosGastos;
            importacion.RowVersion = request.RowVersion;

            // El tipo de cambio sigue a la fecha de póliza; Advalorem/Sobretasa afectan el IGV.
            importacion.TipoCambio = await ImportacionTotalsCalculator.ResolveTipoCambioAsync(_uow, importacion.FechaPoliza, cancellationToken);
            var igvPercentage = await ImportacionTotalsCalculator.GetIgvPercentageAsync(_uow, importacion.FechaPoliza, cancellationToken);

            // Los totales Fob/Flete/Seguro provienen del detalle.
            ImportacionTotalsCalculator.Recalculate(importacion, igvPercentage);

            _uow.Logistica.Transacciones.Importaciones.Update(importacion);

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
