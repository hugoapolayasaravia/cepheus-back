using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.ChangeOrdenTrabajoEstado
{
    /// <summary>
    /// Transiciones válidas del flujo de la OT:
    ///   Pendiente -> EsperaAprobacion | Aprobada
    ///   EsperaAprobacion -> Aprobada | Pendiente
    ///   Aprobada -> EnProceso
    ///   EnProceso -> Concluida
    /// Concluida es un estado final: no admite más transiciones. Al pasar a
    /// Concluida se completa FechaTermino automáticamente si no tenía valor.
    /// </summary>
    public class ChangeOrdenTrabajoEstadoCommandHandler
        : IRequestHandler<ChangeOrdenTrabajoEstadoCommand, OrdenTrabajoResponse>
    {
        private static readonly Dictionary<EstadoOrdenTrabajo, EstadoOrdenTrabajo[]> ValidTransitions = new()
        {
            [EstadoOrdenTrabajo.Pendiente] = new[] { EstadoOrdenTrabajo.EsperaAprobacion, EstadoOrdenTrabajo.Aprobada },
            [EstadoOrdenTrabajo.EsperaAprobacion] = new[] { EstadoOrdenTrabajo.Aprobada, EstadoOrdenTrabajo.Pendiente },
            [EstadoOrdenTrabajo.Aprobada] = new[] { EstadoOrdenTrabajo.EnProceso },
            [EstadoOrdenTrabajo.EnProceso] = new[] { EstadoOrdenTrabajo.Concluida },
            [EstadoOrdenTrabajo.Concluida] = Array.Empty<EstadoOrdenTrabajo>()
        };

        private readonly IUnitOfWork _uow;

        public ChangeOrdenTrabajoEstadoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenTrabajoResponse> Handle(ChangeOrdenTrabajoEstadoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            if (!System.Enum.TryParse<EstadoOrdenTrabajo>(request.NuevoEstado, true, out var nuevoEstado))
            {
                throw new ArgumentException($"Estado '{request.NuevoEstado}' no es válido.");
            }

            var ordenTrabajo = await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query()
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == code, cancellationToken);

            if (ordenTrabajo is null)
            {
                throw new KeyNotFoundException($"Orden de Trabajo {plantaCode}/{code} no encontrada.");
            }

            if (!ValidTransitions[ordenTrabajo.Estado].Contains(nuevoEstado))
            {
                throw new InvalidOperationException(
                    $"No se puede pasar de '{ordenTrabajo.Estado}' a '{nuevoEstado}'. " +
                    $"Transiciones válidas desde '{ordenTrabajo.Estado}': " +
                    $"{string.Join(", ", ValidTransitions[ordenTrabajo.Estado])}.");
            }

            ordenTrabajo.Estado = nuevoEstado;

            if (nuevoEstado == EstadoOrdenTrabajo.Concluida && ordenTrabajo.FechaTermino is null)
            {
                ordenTrabajo.FechaTermino = DateTime.UtcNow;
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return await CreateOrdenTrabajo.CreateOrdenTrabajoCommandHandler.Map(_uow, ordenTrabajo, cancellationToken);
        }
    }
}
