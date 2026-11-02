using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.CreateNoAfectoRetencion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.GetNoAfectoRetencionByKey
{
    public class GetNoAfectoRetencionByKeyQueryHandler : IRequestHandler<GetNoAfectoRetencionByKeyQuery, NoAfectoRetencionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNoAfectoRetencionByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NoAfectoRetencionResponse> Handle(GetNoAfectoRetencionByKeyQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Maestros.NoAfectosRetencion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Ruc == request.Ruc && x.Type == request.Type, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"No afecto a retención {request.Ruc}/{request.Type} no encontrado.");
            }

            return CreateNoAfectoRetencionCommandHandler.Map(entity);
        }
    }
}
