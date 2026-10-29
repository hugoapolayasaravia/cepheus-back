using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.GetTipoSctrByCode
{
    public class GetTipoSctrByCodeQueryHandler : IRequestHandler<GetTipoSctrByCodeQuery, TipoSctrResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoSctrByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoSctrResponse> Handle(GetTipoSctrByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoSctr = await _uow.Rrhh.Catalogos.TiposSctr.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoSctrResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoSctr is null)
            {
                throw new KeyNotFoundException($"Tipo de SCTR {request.Code} no encontrado.");
            }

            return tipoSctr;
        }
    }
}