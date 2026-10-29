using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.CreateListaPrecio;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.GetListaPrecioById
{
    public class GetListaPrecioByIdQueryHandler : IRequestHandler<GetListaPrecioByIdQuery, ListaPrecioResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetListaPrecioByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ListaPrecioResponse> Handle(GetListaPrecioByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.ListasPrecio.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Lista de precio {request.Id} no encontrada.");
            }

            return CreateListaPrecioCommandHandler.Map(entity);
        }
    }
}
