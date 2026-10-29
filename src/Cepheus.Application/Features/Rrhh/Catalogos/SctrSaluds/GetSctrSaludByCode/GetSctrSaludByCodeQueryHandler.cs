using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.GetSctrSaludByCode
{
    public class GetSctrSaludByCodeQueryHandler : IRequestHandler<GetSctrSaludByCodeQuery, SctrSaludResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSctrSaludByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SctrSaludResponse> Handle(GetSctrSaludByCodeQuery request, CancellationToken cancellationToken)
        {
            var sctrSalud = await _uow.Rrhh.Catalogos.SctrsSalud.Query()
                .AsNoTracking()
                .Where(s => s.Code == request.Code)
                .Select(s => new SctrSaludResponse
                {
                    Code = s.Code,
                    Name = s.Name,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    RowVersion = s.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (sctrSalud is null)
            {
                throw new KeyNotFoundException($"Cobertura de salud SCTR {request.Code} no encontrada.");
            }

            return sctrSalud;
        }
    }
}