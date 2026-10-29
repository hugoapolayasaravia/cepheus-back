using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.UpdateCategoriaProducto
{
    public class UpdateCategoriaProductoCommandHandler : IRequestHandler<UpdateCategoriaProductoCommand, CategoriaProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCategoriaProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CategoriaProductoResponse> Handle(UpdateCategoriaProductoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Facturacion.Catalogos.CategoriasProducto.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Categoría de producto {request.Code} no encontrado.");
            }

            var entity = new CategoriaProducto
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                FirthCode = string.IsNullOrWhiteSpace(request.FirthCode) ? null : request.FirthCode.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Catalogos.CategoriasProducto.Update(entity);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateCategoriaProducto.CreateCategoriaProductoCommandHandler.Map(entity);
        }
    }
}
