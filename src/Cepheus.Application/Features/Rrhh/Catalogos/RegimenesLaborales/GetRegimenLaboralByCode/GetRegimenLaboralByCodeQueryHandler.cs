using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.GetRegimenLaboralByCode
{
    public class GetRegimenLaboralByCodeQueryHandler : IRequestHandler<GetRegimenLaboralByCodeQuery, RegimenLaboralResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetRegimenLaboralByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RegimenLaboralResponse> Handle(GetRegimenLaboralByCodeQuery request, CancellationToken cancellationToken)
        {
            var regimenLaboral = await _uow.Rrhh.Catalogos.RegimenesLaborales.Query()
                .AsNoTracking()
                .Where(r => r.Code == request.Code)
                .Select(r => new RegimenLaboralResponse
                {
                    Code = r.Code,
                    Name = r.Name,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    RowVersion = r.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (regimenLaboral is null)
            {
                throw new KeyNotFoundException($"Régimen laboral {request.Code} no encontrado.");
            }

            return regimenLaboral;
        }
    }
}