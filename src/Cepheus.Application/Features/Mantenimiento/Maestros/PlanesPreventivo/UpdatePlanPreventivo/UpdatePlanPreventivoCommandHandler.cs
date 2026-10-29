using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.UpdatePlanPreventivo
{
    public class UpdatePlanPreventivoCommandHandler : IRequestHandler<UpdatePlanPreventivoCommand, PlanPreventivoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlanPreventivoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanPreventivoResponse> Handle(UpdatePlanPreventivoCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var current = await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Code == code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Plan preventivo {plantaCode}/{code} no encontrado.");
            }

            var entity = new PlanMantenimientoPreventivo
            {
                PlantaCode = plantaCode,
                Code = code,
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
                TiempoRestante = request.TiempoRestante,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El plan fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query()
                .AsNoTracking()
                .Where(x => x.PlantaCode == plantaCode && x.Code == code)
                .Select(PlanPreventivoProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
