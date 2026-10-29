using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.CreateCategoriaProducto
{
    public class CreateCategoriaProductoCommandHandler : IRequestHandler<CreateCategoriaProductoCommand, CategoriaProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCategoriaProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CategoriaProductoResponse> Handle(CreateCategoriaProductoCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Facturacion.Catalogos.CategoriasProducto.Query().Select(x => x.Code), length: 3, entityLabel: "Categorías de producto", cancellationToken);

            var entity = new CategoriaProducto
            {
                Code = code,
                Name = request.Name.Trim(),
                FirthCode = string.IsNullOrWhiteSpace(request.FirthCode) ? null : request.FirthCode.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.CategoriasProducto.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static CategoriaProductoResponse Map(CategoriaProducto e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            FirthCode = e.FirthCode,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
