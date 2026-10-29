using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.CopyPlanPreventivo
{
    public class CopyPlanPreventivoCommandHandler : IRequestHandler<CopyPlanPreventivoCommand, PlanPreventivoResponse>
    {
        private const int MaxConcurrencyRetries = 3;

        private readonly IUnitOfWork _uow;

        public CopyPlanPreventivoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanPreventivoResponse> Handle(CopyPlanPreventivoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var source = await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                .AsNoTracking()
                .Include(x => x.ManoObra)
                .Include(x => x.Materiales)
                .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Code == code, cancellationToken);

            if (source is null)
            {
                throw new KeyNotFoundException($"Plan preventivo {plantaCode}/{code} no encontrado.");
            }

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var newCode = await PlanPreventivoCodeGenerator.NextCodeAsync(_uow, plantaCode, cancellationToken);

                var copy = new PlanMantenimientoPreventivo
                {
                    PlantaCode = plantaCode,
                    Code = newCode,
                    Description = source.Description,
                    EquipoCode = source.EquipoCode,
                    EspecialidadCode = source.EspecialidadCode,
                    ResponsableCode = source.ResponsableCode,
                    SubCentroCostoCode = source.SubCentroCostoCode,
                    SubCentroEjecutorCode = source.SubCentroEjecutorCode,
                    ActividadCode = source.ActividadCode,
                    Frecuencia = source.Frecuencia,
                    UnidadFrecuencia = source.UnidadFrecuencia,
                    Factor = source.Factor,
                    // Legacy: la copia limpia Fecha_Ini y Fecha_Ter.
                    FechaInicio = null,
                    FechaTermino = null,
                    FrecuenciaGama = source.FrecuenciaGama,
                    TiempoRestante = source.TiempoRestante
                };

                foreach (var manoObra in source.ManoObra)
                {
                    copy.ManoObra.Add(new PlanMantenimientoPreventivoManoObra
                    {
                        PlantaCode = plantaCode,
                        PlanCode = newCode,
                        TrabajadorCode = manoObra.TrabajadorCode,
                        Tiempo = manoObra.Tiempo,
                        CostoTotal = manoObra.CostoTotal
                    });
                }

                foreach (var material in source.Materiales)
                {
                    copy.Materiales.Add(new PlanMantenimientoPreventivoMaterial
                    {
                        PlantaCode = plantaCode,
                        PlanCode = newCode,
                        ArticuloCode = material.ArticuloCode,
                        Cantidad = material.Cantidad,
                        CostoUnitario = material.CostoUnitario,
                        CostoTotal = material.CostoTotal
                    });
                }

                await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.AddAsync(copy, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);

                    return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                        .AsNoTracking()
                        .Where(x => x.PlantaCode == plantaCode && x.Code == newCode)
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
