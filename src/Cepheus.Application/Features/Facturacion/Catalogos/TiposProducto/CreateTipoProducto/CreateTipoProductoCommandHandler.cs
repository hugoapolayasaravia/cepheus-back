using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.CreateTipoProducto
{
    public class CreateTipoProductoCommandHandler
        : IRequestHandler<CreateTipoProductoCommand, TipoProductoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoProductoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoProductoResponse> Handle(
            CreateTipoProductoCommand request,
            CancellationToken cancellationToken)
        {
            //var code = await SequentialCodeGenerator.NextAsync(
            //    _uow.Facturacion.Catalogos.TiposProducto
            //        .Query()
            //        .Select(t => t.Code),
            //    length: 2,
            //    entityLabel: "Tipos de Producto",
            //    cancellationToken);

            var tipoProducto = new TipoProducto
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.TiposProducto
                .AddAsync(tipoProducto, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoProducto);
        }

        internal static TipoProductoResponse Map(TipoProducto e) => new()
        {
            Code = e.Code,
            Name = e.Name,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}