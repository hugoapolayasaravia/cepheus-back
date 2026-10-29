using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.GetSegmentoVentasByCode
{
    public class GetSegmentoVentasByCodeQueryHandler : IRequestHandler<GetSegmentoVentasByCodeQuery, SegmentoVentasResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetSegmentoVentasByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SegmentoVentasResponse> Handle(GetSegmentoVentasByCodeQuery request, CancellationToken cancellationToken)
        {
            var segmentoVentas = await _uow.Facturacion.Catalogos.SegmentosVentas.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new SegmentoVentasResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (segmentoVentas is null)
            {
                throw new KeyNotFoundException($"Segmento de ventas {request.Code} no encontrado.");
            }

            return segmentoVentas;
        }
    }
}
