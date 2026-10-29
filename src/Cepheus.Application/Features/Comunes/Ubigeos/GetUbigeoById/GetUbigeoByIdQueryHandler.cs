using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.Ubigeos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Ubigeos.GetUbigeoById
{
    public class GetUbigeoByIdQueryHandler : IRequestHandler<GetUbigeoByIdQuery, UbigeoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetUbigeoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UbigeoResponse> Handle(GetUbigeoByIdQuery request, CancellationToken cancellationToken)
        {
            var ubigeo = await _uow.Comunes.Ubigeos.Query()
                .AsNoTracking()
                .Where(u => u.Code == request.Code)
                .Select(u => new UbigeoResponse
                {
                    Code = u.Code,
                    Department = u.Department,
                    Province = u.Province,
                    District = u.District,
                    FullAddress = u.FullAddress,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedBy = u.UpdatedBy,
                    RowVersion = u.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (ubigeo is null)
            {
                throw new KeyNotFoundException($"Ubigeo {request.Code} no encontrado.");
            }

            return ubigeo;
        }
    }
}
