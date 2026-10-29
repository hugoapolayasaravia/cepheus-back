using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.UpdatePlanPreventivoManoObra
{
    public class UpdatePlanPreventivoManoObraCommandHandler : IRequestHandler<UpdatePlanPreventivoManoObraCommand, PlanPreventivoManoObraResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlanPreventivoManoObraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanPreventivoManoObraResponse> Handle(UpdatePlanPreventivoManoObraCommand request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var plan = request.PlanCode.Trim().ToUpperInvariant();
            var key = request.TrabajadorCode.Trim().ToUpperInvariant();

            var current = await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoManoObra.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.PlantaCode == planta && d.PlanCode == plan && d.TrabajadorCode == key, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Registro {planta}/{plan}/{key} no encontrado.");
            }

            var entity = new PlanMantenimientoPreventivoManoObra
            {
                PlantaCode = planta,
                PlanCode = plan,
                TrabajadorCode = key,
                Tiempo = request.Tiempo,
                CostoTotal = request.CostoTotal,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoManoObra.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoManoObra.Query()
                .AsNoTracking()
                .Where(d => d.PlantaCode == planta && d.PlanCode == plan && d.TrabajadorCode == key)
                .Select(PlanPreventivoManoObraProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
