using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.GetCotizacionMetradoResumenesByCotizacion
{
    public class GetCotizacionMetradoResumenesByCotizacionQueryHandler
        : IRequestHandler<GetCotizacionMetradoResumenesByCotizacionQuery, List<CotizacionMetradoResumenResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCotizacionMetradoResumenesByCotizacionQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<CotizacionMetradoResumenResponse>> Handle(
            GetCotizacionMetradoResumenesByCotizacionQuery request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var resumenes = await _uow.Facturacion.Transacciones.CotizacionesMetradoResumen.Query()
                .AsNoTracking()
                .Where(r => r.NegocioCode == negocio && r.Year == request.Year
                         && r.Month == request.Month && r.Code == code)
                .OrderBy(r => r.LevelNumber)
                .ToListAsync(cancellationToken);

            return resumenes.Select(r => new CotizacionMetradoResumenResponse
            {
                NegocioCode = r.NegocioCode,
                Year = r.Year,
                Month = r.Month,
                Code = r.Code,
                LevelNumber = r.LevelNumber,
                LevelName = r.LevelName,
                AlturaLosaCode = r.AlturaLosaCode,
                OverloadOrShortage = r.OverloadOrShortage,
                LinealMeters = r.LinealMeters,
                TotalVaults = r.TotalVaults,
                TotalMeters = r.TotalMeters,
                TotalPrice = r.TotalPrice,
                PricePerM2 = r.PricePerM2,
                Quantity = r.Quantity,
                BuildingLevel = r.BuildingLevel,
                HasTransport = r.HasTransport,
                TotalVaultsAlt = r.TotalVaultsAlt,
                TotalMetersAlt = r.TotalMetersAlt,
                TotalPriceAlt = r.TotalPriceAlt,
                PricePerM2Alt = r.PricePerM2Alt,
                HasMinPrice = r.HasMinPrice,
                MinTotal = r.MinTotal,
                MinTotalAlt = r.MinTotalAlt,
                MinTotalB = r.MinTotalB,
                MinTotalAltB = r.MinTotalAltB,
                HasTransportB = r.HasTransportB,
                HasMinPriceB = r.HasMinPriceB,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                RowVersion = r.RowVersion
            }).ToList();
        }
    }
}
