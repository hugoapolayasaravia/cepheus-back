using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Areas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.GetAreaByCode
{
    public class GetAreaByCodeQueryHandler : IRequestHandler<GetAreaByCodeQuery, AreaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetAreaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AreaResponse> Handle(GetAreaByCodeQuery request, CancellationToken cancellationToken)
        {
            var area = await _uow.Rrhh.Catalogos.Areas.Query()
                .AsNoTracking()
                .Where(a => a.Code == request.Code)
                .Select(a => new AreaResponse
                {
                    Code = a.Code,
                    Name = a.Name,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    RowVersion = a.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (area is null)
            {
                throw new KeyNotFoundException($"Área {request.Code} no encontrada.");
            }

            return area;
        }
    }
}