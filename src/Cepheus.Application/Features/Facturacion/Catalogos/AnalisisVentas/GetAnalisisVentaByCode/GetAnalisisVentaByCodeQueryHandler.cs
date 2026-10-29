using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.GetAnalisisVentaByCode
{
    public class GetAnalisisVentaByCodeQueryHandler : IRequestHandler<GetAnalisisVentaByCodeQuery, AnalisisVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetAnalisisVentaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AnalisisVentaResponse> Handle(GetAnalisisVentaByCodeQuery request, CancellationToken cancellationToken)
        {
            var analisisVenta = await _uow.Facturacion.Catalogos.AnalisisVentas.Query()
                .AsNoTracking()
                .Where(a => a.Code == request.Code)
                .Select(a => new AnalisisVentaResponse
                {
                    Code = a.Code,
                    Name = a.Name,
                    ShortName = a.ShortName,
                    SegmentoVentasCode = a.SegmentoVentasCode,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt,
                    RowVersion = a.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (analisisVenta is null)
            {
                throw new KeyNotFoundException($"Análisis de ventas {request.Code} no encontrado.");
            }

            return analisisVenta;
        }
    }
}
