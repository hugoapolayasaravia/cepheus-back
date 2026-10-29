using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.CreatePlanPreventivo
{
    public class CreatePlanPreventivoCommandHandler : IRequestHandler<CreatePlanPreventivoCommand, PlanPreventivoResponse>
    {
        private const int MaxConcurrencyRetries = 3;

        private readonly IUnitOfWork _uow;

        public CreatePlanPreventivoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanPreventivoResponse> Handle(CreatePlanPreventivoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await PlanPreventivoCodeGenerator.NextCodeAsync(_uow, plantaCode, cancellationToken);

                var entity = new PlanMantenimientoPreventivo
                {
                    PlantaCode = plantaCode,
                    Code = nextCode,
                    Description = request.Description.Trim(),
                    EquipoCode = request.EquipoCode.Trim().ToUpperInvariant(),
                    EspecialidadCode = request.EspecialidadCode.Trim().ToUpperInvariant(),
                    ResponsableCode = request.ResponsableCode.Trim().ToUpperInvariant(),
                    SubCentroCostoCode = request.SubCentroCostoCode.Trim().ToUpperInvariant(),
                    SubCentroEjecutorCode = request.SubCentroEjecutorCode.Trim().ToUpperInvariant(),
                    ActividadCode = request.ActividadCode.Trim().ToUpperInvariant(),
                    Frecuencia = request.Frecuencia,
                    UnidadFrecuencia = request.UnidadFrecuencia,
                    Factor = request.Factor,
                    FechaInicio = request.FechaInicio?.Date,
                    FechaTermino = request.FechaTermino?.Date,
                    FrecuenciaGama = request.FrecuenciaGama,
                    TiempoRestante = request.TiempoRestante
                };

                await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.AddAsync(entity, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);

                    return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                        .AsNoTracking()
                        .Where(x => x.PlantaCode == plantaCode && x.Code == nextCode)
                        .Select(PlanPreventivoProjection.ToResponse)
                        .FirstAsync(cancellationToken);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                    _uow.ClearTracking();
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo del plan preventivo para la planta {plantaCode} por alta concurrencia. Intente nuevamente.");
        }
    }
}
