using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Tramites.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.GetTramiteByCode
{
    public class GetTramiteByCodeQueryHandler : IRequestHandler<GetTramiteByCodeQuery, TramiteResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTramiteByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TramiteResponse> Handle(GetTramiteByCodeQuery request, CancellationToken cancellationToken)
        {
            var tramite = await _uow.Logistica.Catalogos.Tramites.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TramiteResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    CreatedBy = t.CreatedBy,
                    UpdatedBy = t.UpdatedBy,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tramite is null)
            {
                throw new KeyNotFoundException($"Trámite {request.Code} no encontrado.");
            }

            return tramite;
        }
    }
}