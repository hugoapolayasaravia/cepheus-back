using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.GetTipoAfiliacionByCode
{
    public class GetTipoAfiliacionByCodeQueryHandler : IRequestHandler<GetTipoAfiliacionByCodeQuery, TipoAfiliacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoAfiliacionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoAfiliacionResponse> Handle(GetTipoAfiliacionByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoAfiliacion = await _uow.Rrhh.Catalogos.TiposAfiliacion.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoAfiliacionResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoAfiliacion is null)
            {
                throw new KeyNotFoundException($"Tipo de afiliación {request.Code} no encontrado.");
            }

            return tipoAfiliacion;
        }
    }
}