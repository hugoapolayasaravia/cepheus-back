using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.CreateTipoCompra
{
    public class CreateTipoCompraCommandHandler : IRequestHandler<CreateTipoCompraCommand, TipoCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoCompraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCompraResponse> Handle(CreateTipoCompraCommand request, CancellationToken cancellationToken)
        {

            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.TiposCompra.Query().Select(f => f.Code), length: 1, entityLabel: "Tipos de Compra", cancellationToken);

            var tipoCompra = new TipoCompra
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.TiposCompra.AddAsync(tipoCompra, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoCompra);
        }

        internal static TipoCompraResponse Map(TipoCompra tipoCompra) => new()
        {
            Code = tipoCompra.Code,
            Name = tipoCompra.Name,
            IsActive = tipoCompra.IsActive,
            CreatedAt = tipoCompra.CreatedAt,
            UpdatedAt = tipoCompra.UpdatedAt,
            CreatedBy = tipoCompra.CreatedBy,
            UpdatedBy = tipoCompra.UpdatedBy,
            RowVersion = tipoCompra.RowVersion
        };
    }
}