using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.GetNivelByCode
{
    public class GetNivelByCodeQueryHandler : IRequestHandler<GetNivelByCodeQuery, NivelResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNivelByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NivelResponse> Handle(GetNivelByCodeQuery request, CancellationToken cancellationToken)
        {
            var nivel = await _uow.Logistica.Catalogos.Niveles.Query()
                .AsNoTracking()
                .Where(n => n.Code == request.Code)
                .Select(n => new NivelResponse
                {
                    Code = n.Code,
                    Name = n.Name,
                    FechaInicio = n.FechaInicio,
                    FechaFin = n.FechaFin,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    CreatedBy = n.CreatedBy,
                    UpdatedBy = n.UpdatedBy,
                    RowVersion = n.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (nivel is null)
            {
                throw new KeyNotFoundException($"Nivel {request.Code} no encontrado.");
            }

            return nivel;
        }
    }
}
