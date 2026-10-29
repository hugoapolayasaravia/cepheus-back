using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.CreateTipoOperacion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.GetTipoOperacionById
{
    public class GetTipoOperacionByIdQueryHandler : IRequestHandler<GetTipoOperacionByIdQuery, TipoOperacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoOperacionByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoOperacionResponse> Handle(GetTipoOperacionByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.TiposOperacion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de operación {request.Code} no encontrado.");
            }

            return CreateTipoOperacionCommandHandler.Map(entity);
        }
    }
}
