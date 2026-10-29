using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.UpdateOrdenTrabajo
{
    public class UpdateOrdenTrabajoCommandHandler : IRequestHandler<UpdateOrdenTrabajoCommand, OrdenTrabajoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOrdenTrabajoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenTrabajoResponse> Handle(UpdateOrdenTrabajoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var current = await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Orden de Trabajo {plantaCode}/{code} no encontrada.");
            }

            var ordenTrabajo = new OrdenTrabajo
            {
                PlantaCode = plantaCode,
                Code = code,
                Description = request.Description.Trim(),

                FechaProceso = request.FechaProceso,
                FechaTermino = request.FechaTermino,

                ResponsableCode = request.ResponsableCode.Trim().ToUpperInvariant(),

                EspecialidadCode = request.EspecialidadCode.Trim().ToUpperInvariant(),
                OportunidadCode = request.OportunidadCode.Trim().ToUpperInvariant(),
                EquipoCode = request.EquipoCode.Trim().ToUpperInvariant(),
                PrioridadCode = request.PrioridadCode.Trim().ToUpperInvariant(),
                InspeccionCode = request.InspeccionCode.Trim().ToUpperInvariant(),
                TipoOrdenCode = request.TipoOrdenCode.Trim().ToUpperInvariant(),
                ActividadCode = request.ActividadCode.Trim().ToUpperInvariant(),

                SubCentroCostoCode = Normalize(request.SubCentroCostoCode),
                SubCentroEjecutorCode = Normalize(request.SubCentroEjecutorCode),
                PlanMantenimientoPreventivoCode = Normalize(request.PlanMantenimientoPreventivoCode),

                DowntimeHours = request.DowntimeHours,
                Estado = current.Estado,
                Horometro = request.Horometro,
                Observaciones = request.Observations?.Trim(),
                Turno = System.Enum.Parse<Turno>(request.Turno, ignoreCase: true),

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Update(ordenTrabajo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La Orden de Trabajo fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await CreateOrdenTrabajo.CreateOrdenTrabajoCommandHandler.Map(_uow, ordenTrabajo, cancellationToken);
        }

        private static string? Normalize(string? code)
            => string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant();
    }
}
