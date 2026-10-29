using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.UpdatePlanArticulo
{
    public class UpdatePlanArticuloCommandHandler : IRequestHandler<UpdatePlanArticuloCommand, PlanArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePlanArticuloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PlanArticuloResponse> Handle(UpdatePlanArticuloCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.PlanesArticulo.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Plan de artículo {request.Code} no encontrado.");
            }

            var plan = new PlanArticulo
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.PlanesArticulo.Update(plan);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El plan de artículo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new PlanArticuloResponse
            {
                Code = plan.Code,
                Name = plan.Name,
                IsActive = plan.IsActive,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt,
                CreatedBy = plan.CreatedBy,
                UpdatedBy = plan.UpdatedBy,
                RowVersion = plan.RowVersion
            };
        }
    }
}