using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.GetTipoViaByCode
{
    public class GetTipoViaByCodeQueryHandler : IRequestHandler<GetTipoViaByCodeQuery, TipoViaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoViaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoViaResponse> Handle(GetTipoViaByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoVia = await _uow.Rrhh.Catalogos.TiposVia.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoViaResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    Abbreviation = t.Abbreviation,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoVia is null)
            {
                throw new KeyNotFoundException($"Tipo de vía {request.Code} no encontrado.");
            }

            return tipoVia;
        }
    }
}