using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.GetSctrPensionByCode
{
    public class GetSctrPensionByCodeQueryHandler : IRequestHandler<GetSctrPensionByCodeQuery, SctrPensionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSctrPensionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SctrPensionResponse> Handle(GetSctrPensionByCodeQuery request, CancellationToken cancellationToken)
        {
            var sctrPension = await _uow.Rrhh.Catalogos.SctrsPension.Query()
                .AsNoTracking()
                .Where(s => s.Code == request.Code)
                .Select(s => new SctrPensionResponse
                {
                    Code = s.Code,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (sctrPension is null)
            {
                throw new KeyNotFoundException($"Cobertura de pensión SCTR {request.Code} no encontrada.");
            }

            return sctrPension;
        }
    }
}