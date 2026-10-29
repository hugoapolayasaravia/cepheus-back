using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.GetFamiliaByCode
{
    public class GetFamiliaByCodeQueryHandler : IRequestHandler<GetFamiliaByCodeQuery, FamiliaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetFamiliaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FamiliaResponse> Handle(GetFamiliaByCodeQuery request, CancellationToken cancellationToken)
        {
            var familia = await _uow.Logistica.Catalogos.Familias.Query()
                .AsNoTracking()
                .Where(f => f.Code == request.Code)
                .Select(f => new FamiliaResponse
                {
                    Code = f.Code,
                    Name = f.Name,
                    IsActive = f.IsActive,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    CreatedBy = f.CreatedBy,
                    UpdatedBy = f.UpdatedBy,
                    RowVersion = f.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (familia is null)
            {
                throw new KeyNotFoundException($"Familia {request.Code} no encontrada.");
            }

            return familia;
        }
    }
}