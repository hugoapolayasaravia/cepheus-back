using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.CreateCategoriaProducto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.GetCategoriaProductoByCode
{
    public class GetCategoriaProductoByCodeQueryHandler : IRequestHandler<GetCategoriaProductoByCodeQuery, CategoriaProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCategoriaProductoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CategoriaProductoResponse> Handle(GetCategoriaProductoByCodeQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.CategoriasProducto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Categoría de producto {request.Code} no encontrado.");
            }

            return CreateCategoriaProductoCommandHandler.Map(entity);
        }
    }
}
