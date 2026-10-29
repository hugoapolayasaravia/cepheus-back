using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.Common;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.CreateUnidadMedidaVenta;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.GetUnidadMedidaVentaById
{
    public class GetUnidadMedidaVentaByIdQueryHandler : IRequestHandler<GetUnidadMedidaVentaByIdQuery, UnidadMedidaVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetUnidadMedidaVentaByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadMedidaVentaResponse> Handle(GetUnidadMedidaVentaByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _uow.Facturacion.Catalogos.UnidadesMedidaVenta.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Unidad de medida de venta {request.Code} no encontrado.");
            }

            return CreateUnidadMedidaVentaCommandHandler.Map(entity);
        }
    }
}
