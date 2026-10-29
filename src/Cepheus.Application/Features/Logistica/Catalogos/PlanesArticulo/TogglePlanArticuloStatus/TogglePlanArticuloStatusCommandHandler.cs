using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.TogglePlanArticuloStatus
{
    public class TogglePlanArticuloStatusCommandHandler : IRequestHandler<TogglePlanArticuloStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;

        public TogglePlanArticuloStatusCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> Handle(TogglePlanArticuloStatusCommand request, CancellationToken cancellationToken)
        {
            var plan = await _uow.Logistica.Catalogos.PlanesArticulo.Query()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (plan is null)
            {
                throw new KeyNotFoundException($"Plan de artículo {request.Code} no encontrado.");
            }

            plan.IsActive = !plan.IsActive;

            await _uow.SaveChangesAsync(cancellationToken);

            return plan.IsActive;
        }
    }
}