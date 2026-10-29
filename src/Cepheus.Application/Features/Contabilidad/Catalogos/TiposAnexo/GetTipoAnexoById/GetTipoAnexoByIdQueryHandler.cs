using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.CreateTipoAnexo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.GetTipoAnexoById
{
    public class GetTipoAnexoByIdQueryHandler : IRequestHandler<GetTipoAnexoByIdQuery, TipoAnexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoAnexoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoAnexoResponse> Handle(GetTipoAnexoByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Catalogos.TiposAnexo.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de anexo {request.Code} no encontrado.");
            }

            return CreateTipoAnexoCommandHandler.Map(entity);
        }
    }
}
