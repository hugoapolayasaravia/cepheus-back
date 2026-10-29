using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using Cepheus.Domain.Mantenimiento.Enum;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.CreateOrdenTrabajo
{
    public class CreateOrdenTrabajoCommandHandler : IRequestHandler<CreateOrdenTrabajoCommand, OrdenTrabajoResponse>
    {
        private const int MaxConcurrencyRetries = 3;
        private const int CodeLength = 6;

        private readonly IUnitOfWork _uow;

        public CreateOrdenTrabajoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenTrabajoResponse> Handle(CreateOrdenTrabajoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(plantaCode, cancellationToken);

                var ordenTrabajo = new OrdenTrabajo
                {
                    PlantaCode = plantaCode,
                    Code = nextCode,
                    Description = request.Description.Trim(),

                    FechaProceso = request.FechaProceso,

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

                    DowntimeHours = 0,
                    Estado = EstadoOrdenTrabajo.Pendiente,
                    Horometro = request.Horometro,
                    Observaciones = request.Observations?.Trim(),
                    Turno = System.Enum.Parse<Turno>(request.Turno, ignoreCase: true)
                };

                await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.AddAsync(ordenTrabajo, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return await Map(_uow, ordenTrabajo, cancellationToken);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                    _uow.ClearTracking();
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo de Orden de Trabajo para la planta {plantaCode} por alta concurrencia. Intente nuevamente.");
        }

        private async Task<string> NextCodeAsync(string plantaCode, CancellationToken cancellationToken)
        {
            var lastCode = await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query()
                .Where(o => o.PlantaCode == plantaCode)
                .OrderByDescending(o => o.Code)
                .Select(o => o.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (lastCode is not null && int.TryParse(lastCode, out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return next.ToString().PadLeft(CodeLength, '0');
        }

        private static string? Normalize(string? code)
            => string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant();

        internal static async Task<OrdenTrabajoResponse> Map(IUnitOfWork uow, OrdenTrabajo o, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            return new OrdenTrabajoResponse
            {
                PlantaCode = o.PlantaCode,
                Code = o.Code,
                Description = o.Description,
                FechaProceso = o.FechaProceso,
                FechaTermino = o.FechaTermino,
                ResponsableCode = o.ResponsableCode,
                EspecialidadCode = o.EspecialidadCode,
                OportunidadCode = o.OportunidadCode,
                EquipoCode = o.EquipoCode,
                PrioridadCode = o.PrioridadCode,
                InspeccionCode = o.InspeccionCode,
                TipoOrdenCode = o.TipoOrdenCode,
                ActividadCode = o.ActividadCode,
                SubCentroCostoCode = o.SubCentroCostoCode,
                SubCentroEjecutorCode = o.SubCentroEjecutorCode,
                PlanMantenimientoPreventivoCode = o.PlanMantenimientoPreventivoCode,
                DowntimeHours = o.DowntimeHours,
                Estado = o.Estado.ToString(),
                Horometro = o.Horometro,
                Observations = o.Observaciones,
                Turno = o.Turno.ToString(),
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                RowVersion = o.RowVersion
            };
        }
    }
}
