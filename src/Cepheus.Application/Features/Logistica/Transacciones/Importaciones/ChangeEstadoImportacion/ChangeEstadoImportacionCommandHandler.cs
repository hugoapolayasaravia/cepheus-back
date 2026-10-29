// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/ChangeEstadoImportacion/ChangeEstadoImportacionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.ChangeEstadoImportacion
{
    /// <summary>
    /// Cambio manual de estado: solo Pendiente -> Anulado.
    /// Procesado lo establece únicamente la generación de la Nota de Ingreso (con ingreso a stock).
    /// Una importación Procesada se anula anulando sus Notas de Ingreso: eso revierte el stock
    /// (solo si no se movió) y la devuelve a Pendiente; recién ahí se puede anular.
    /// </summary>
    public class ChangeEstadoImportacionCommandHandler : IRequestHandler<ChangeEstadoImportacionCommand, ImportacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public ChangeEstadoImportacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ImportacionResponse> Handle(ChangeEstadoImportacionCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            if (!System.Enum.TryParse<EstadoImportacion>(request.NuevoEstado, true, out var nuevoEstado))
            {
                throw new ArgumentException($"Estado '{request.NuevoEstado}' no es válido.");
            }

            var importacion = await _uow.Logistica.Transacciones.Importaciones.Query()
                .FirstOrDefaultAsync(i => i.PlantaCode == plantaCode && i.Code == code, cancellationToken);

            if (importacion is null)
            {
                throw new KeyNotFoundException($"Importación {plantaCode}/{code} no encontrada.");
            }

            if (importacion.Estado != EstadoImportacion.Pendiente || nuevoEstado != EstadoImportacion.Anulado)
            {
                throw new InvalidOperationException(
                    $"No se puede pasar de '{importacion.Estado}' a '{nuevoEstado}'. " +
                    "Solo se permite Pendiente -> Anulado. Procesado lo establece la generación de la Nota de Ingreso; " +
                    "para anular una importación procesada, anule primero sus Notas de Ingreso (eso revierte el stock y la deja Pendiente).");
            }

            importacion.Estado = nuevoEstado;

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
