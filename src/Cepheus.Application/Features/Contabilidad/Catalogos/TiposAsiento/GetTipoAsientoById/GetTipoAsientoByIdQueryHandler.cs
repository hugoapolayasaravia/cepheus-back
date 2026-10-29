using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.Common;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.CreateTipoAsiento;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.GetTipoAsientoById
{
    public class GetTipoAsientoByIdQueryHandler : IRequestHandler<GetTipoAsientoByIdQuery, TipoAsientoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoAsientoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoAsientoResponse> Handle(GetTipoAsientoByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Contabilidad.Catalogos.TiposAsiento.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de asiento {request.Code} no encontrado.");
            }

            return CreateTipoAsientoCommandHandler.Map(entity);
        }
    }
}
