using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.GetTipoZonaByCode
{
    public class GetTipoZonaByCodeQueryHandler : IRequestHandler<GetTipoZonaByCodeQuery, TipoZonaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoZonaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoZonaResponse> Handle(GetTipoZonaByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoZona = await _uow.Rrhh.Catalogos.TiposZona.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoZonaResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoZona is null)
            {
                throw new KeyNotFoundException($"Tipo de zona {request.Code} no encontrado.");
            }

            return tipoZona;
        }
    }
}