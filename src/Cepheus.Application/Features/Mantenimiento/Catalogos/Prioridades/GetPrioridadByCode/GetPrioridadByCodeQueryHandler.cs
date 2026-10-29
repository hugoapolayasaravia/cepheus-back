using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.GetPrioridadByCode
{
    public class GetPrioridadByCodeQueryHandler
        : IRequestHandler<GetPrioridadByCodeQuery, PrioridadResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetPrioridadByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PrioridadResponse> Handle(
            GetPrioridadByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var prioridad = await _uow.Mantenimiento.Catalogos.Prioridades.Query()
                .AsNoTracking()
                .Where(p => p.Code == request.Code)
                .Select(p => new PrioridadResponse
                {
                    Code = p.Code,
                    Name = p.Name,
                    IsActive = p.IsActive,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    RowVersion = p.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (prioridad is null)
            {
                throw new KeyNotFoundException($"Prioridad {request.Code} no encontrada.");
            }

            return prioridad;
        }
    }
}
