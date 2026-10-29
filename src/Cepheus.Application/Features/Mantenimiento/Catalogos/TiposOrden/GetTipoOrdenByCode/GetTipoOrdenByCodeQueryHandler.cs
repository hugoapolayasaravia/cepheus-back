using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.GetTipoOrdenByCode
{
    public class GetTipoOrdenByCodeQueryHandler
        : IRequestHandler<GetTipoOrdenByCodeQuery, TipoOrdenResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoOrdenByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoOrdenResponse> Handle(
            GetTipoOrdenByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var tipoOrden = await _uow.Mantenimiento.Catalogos.TiposOrden.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoOrdenResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoOrden is null)
            {
                throw new KeyNotFoundException($"Tipo de orden {request.Code} no encontrado.");
            }

            return tipoOrden;
        }
    }
}
