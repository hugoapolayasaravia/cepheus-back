using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.GetPlanArticuloByCode
{
    public class GetPlanArticuloByCodeQueryHandler : IRequestHandler<GetPlanArticuloByCodeQuery, PlanArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetPlanArticuloByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanArticuloResponse> Handle(GetPlanArticuloByCodeQuery request, CancellationToken cancellationToken)
        {
            var plan = await _uow.Logistica.Catalogos.PlanesArticulo.Query()
                .AsNoTracking()
                .Where(p => p.Code == request.Code)
                .Select(p => new PlanArticuloResponse
                {
                    Code = p.Code,
                    Name = p.Name,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    CreatedBy = p.CreatedBy,
                    UpdatedBy = p.UpdatedBy,
                    RowVersion = p.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (plan is null)
            {
                throw new KeyNotFoundException($"Plan de artículo {request.Code} no encontrado.");
            }

            return plan;
        }
    }
}