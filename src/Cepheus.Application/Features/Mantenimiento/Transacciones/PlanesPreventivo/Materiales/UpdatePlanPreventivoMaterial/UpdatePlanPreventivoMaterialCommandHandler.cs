using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.Materiales.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.PlanesPreventivo.Materiales.UpdatePlanPreventivoMaterial
{
    public class UpdatePlanPreventivoMaterialCommandHandler : IRequestHandler<UpdatePlanPreventivoMaterialCommand, PlanPreventivoMaterialResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlanPreventivoMaterialCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanPreventivoMaterialResponse> Handle(UpdatePlanPreventivoMaterialCommand request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var plan = request.PlanCode.Trim().ToUpperInvariant();
            var key = request.ArticuloCode.Trim().ToUpperInvariant();

            var current = await _uow.Mantenimiento.Transacciones.PlanesMantenimientoPreventivoMateriales.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.PlantaCode == planta && d.PlanCode == plan && d.ArticuloCode == key, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Registro {planta}/{plan}/{key} no encontrado.");
            }

            var entity = new PlanMantenimientoPreventivoMaterial
            {
                PlantaCode = planta,
                PlanCode = plan,
                ArticuloCode = key,
                Cantidad = request.Cantidad,
                CostoUnitario = request.CostoUnitario,
                CostoTotal = request.CostoTotal,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Transacciones.PlanesMantenimientoPreventivoMateriales.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return await _uow.Mantenimiento.Transacciones.PlanesMantenimientoPreventivoMateriales.Query()
                .AsNoTracking()
                .Where(d => d.PlantaCode == planta && d.PlanCode == plan && d.ArticuloCode == key)
                .Select(PlanPreventivoMaterialProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
