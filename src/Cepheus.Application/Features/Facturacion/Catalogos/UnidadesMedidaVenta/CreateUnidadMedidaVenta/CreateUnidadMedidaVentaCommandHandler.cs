using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.Common;
using Cepheus.Domain.Facturacion.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.CreateUnidadMedidaVenta
{
    public class CreateUnidadMedidaVentaCommandHandler : IRequestHandler<CreateUnidadMedidaVentaCommand, UnidadMedidaVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadMedidaVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadMedidaVentaResponse> Handle(CreateUnidadMedidaVentaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim().ToUpperInvariant();

            var entity = new UnidadMedidaVenta
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Facturacion.Catalogos.UnidadesMedidaVenta.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static UnidadMedidaVentaResponse Map(UnidadMedidaVenta e) => new()
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
