using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.CreateAnexo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.GetAnexoByCode
{
    public class GetAnexoByCodeQueryHandler : IRequestHandler<GetAnexoByCodeQuery, AnexoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetAnexoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AnexoResponse> Handle(GetAnexoByCodeQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Maestros.Anexos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TipoAnexoCode == request.TipoAnexoCode && x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Anexo {request.TipoAnexoCode}/{request.Code} no encontrado.");
            }

            return CreateAnexoCommandHandler.Map(entity);
        }
    }
}
