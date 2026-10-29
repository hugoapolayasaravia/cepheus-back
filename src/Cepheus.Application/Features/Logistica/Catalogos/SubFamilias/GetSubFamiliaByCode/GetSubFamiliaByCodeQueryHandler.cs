using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.GetSubFamiliaByCode
{
    public class GetSubFamiliaByCodeQueryHandler : IRequestHandler<GetSubFamiliaByCodeQuery, SubFamiliaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSubFamiliaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubFamiliaResponse> Handle(GetSubFamiliaByCodeQuery request, CancellationToken cancellationToken)
        {
            var subFamilia = await _uow.Logistica.Catalogos.SubFamilias.Query()
                .AsNoTracking()
                .Where(s => s.Code == request.Code)
                .Select(s => new SubFamiliaResponse
                {
                    Code = s.Code,
                    FamiliaCode = s.FamiliaCode,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    CreatedBy = s.CreatedBy,
                    UpdatedBy = s.UpdatedBy,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (subFamilia is null)
            {
                throw new KeyNotFoundException($"SubFamilia {request.Code} no encontrada.");
            }

            return subFamilia;
        }
    }
}