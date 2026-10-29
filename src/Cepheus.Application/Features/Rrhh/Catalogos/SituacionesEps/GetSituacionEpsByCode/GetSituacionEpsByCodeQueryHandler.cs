using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.GetSituacionEpsByCode
{
    public class GetSituacionEpsByCodeQueryHandler : IRequestHandler<GetSituacionEpsByCodeQuery, SituacionEpsResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSituacionEpsByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SituacionEpsResponse> Handle(GetSituacionEpsByCodeQuery request, CancellationToken cancellationToken)
        {
            var situacionEps = await _uow.Rrhh.Catalogos.SituacionesEps.Query()
                .AsNoTracking()
                .Where(s => s.Code == request.Code)
                .Select(s => new SituacionEpsResponse
                {
                    Code = s.Code,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (situacionEps is null)
            {
                throw new KeyNotFoundException($"Situación EPS {request.Code} no encontrada.");
            }

            return situacionEps;
        }
    }
}