using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Afps.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.GetAfpByCode
{
    public class GetAfpByCodeQueryHandler : IRequestHandler<GetAfpByCodeQuery, AfpResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetAfpByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AfpResponse> Handle(GetAfpByCodeQuery request, CancellationToken cancellationToken)
        {
            var afp = await _uow.Rrhh.Catalogos.Afps.Query()
                .AsNoTracking()
                .Where(a => a.Code == request.Code)
                .Select(a => new AfpResponse
                {
                    Code = a.Code,
                    Name = a.Name,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    RowVersion = a.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (afp is null)
            {
                throw new KeyNotFoundException($"AFP {request.Code} no encontrada.");
            }

            return afp;
        }
    }
}