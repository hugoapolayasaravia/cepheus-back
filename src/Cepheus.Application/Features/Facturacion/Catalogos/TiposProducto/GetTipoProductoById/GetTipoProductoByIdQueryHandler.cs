using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.CreateTipoProducto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.GetTipoProductoById
{
    public class GetTipoProductoByIdQueryHandler : IRequestHandler<GetTipoProductoByIdQuery, TipoProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoProductoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoProductoResponse> Handle(GetTipoProductoByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.TiposProducto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Tipo de producto {request.Code} no encontrado.");
            }

            return CreateTipoProductoCommandHandler.Map(entity);
        }
    }
}
