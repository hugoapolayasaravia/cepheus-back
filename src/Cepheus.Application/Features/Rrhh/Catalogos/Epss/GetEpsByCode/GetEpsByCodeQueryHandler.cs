using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Epss.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.GetEpsByCode
{
    public class GetEpsByCodeQueryHandler : IRequestHandler<GetEpsByCodeQuery, EpsResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetEpsByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<EpsResponse> Handle(GetEpsByCodeQuery request, CancellationToken cancellationToken)
        {
            var eps = await _uow.Rrhh.Catalogos.Epss.Query()
                .AsNoTracking()
                .Where(e => e.Code == request.Code)
                .Select(e => new EpsResponse
                {
                    Code = e.Code,
                    Name = e.Name,
                    IsActive = e.IsActive,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    RowVersion = e.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (eps is null)
            {
                throw new KeyNotFoundException($"EPS {request.Code} no encontrada.");
            }

            return eps;
        }
    }
}