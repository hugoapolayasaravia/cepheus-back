using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.CreatePlanPreventivoMaterial
{
    public class CreatePlanPreventivoMaterialCommandHandler : IRequestHandler<CreatePlanPreventivoMaterialCommand, PlanPreventivoMaterialResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePlanPreventivoMaterialCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanPreventivoMaterialResponse> Handle(CreatePlanPreventivoMaterialCommand request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode.Trim().ToUpperInvariant();
            var plan = request.PlanCode.Trim().ToUpperInvariant();
            var key = request.ArticuloCode.Trim().ToUpperInvariant();

            var entity = new PlanMantenimientoPreventivoMaterial
            {
                PlantaCode = planta,
                PlanCode = plan,
                ArticuloCode = key,
                Cantidad = request.Cantidad,
                CostoUnitario = request.CostoUnitario,
                CostoTotal = request.CostoTotal
            };

            await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoMateriales.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoMateriales.Query()
                .AsNoTracking()
                .Where(d => d.PlantaCode == planta && d.PlanCode == plan && d.ArticuloCode == key)
                .Select(PlanPreventivoMaterialProjection.ToResponse)
                .FirstAsync(cancellationToken);
        }
    }
}
