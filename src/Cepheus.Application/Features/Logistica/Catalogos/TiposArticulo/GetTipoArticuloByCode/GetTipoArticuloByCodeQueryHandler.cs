    using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.GetTipoArticuloByCode
{
    public class GetTipoArticuloByCodeQueryHandler : IRequestHandler<GetTipoArticuloByCodeQuery, TipoArticuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoArticuloByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoArticuloResponse> Handle(GetTipoArticuloByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoArticulo = await _uow.Logistica.Catalogos.TiposArticulo.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoArticuloResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    Description = t.Description,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    CreatedBy = t.CreatedBy,
                    UpdatedBy = t.UpdatedBy,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoArticulo is null)
            {
                throw new KeyNotFoundException($"Tipo de artículo {request.Code} no encontrado.");
            }

            return tipoArticulo;
        }
    }
}