using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.Common;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.CreateCotizacionMetradoResumen
{
    public class CreateCotizacionMetradoResumenCommandHandler
        : IRequestHandler<CreateCotizacionMetradoResumenCommand, CotizacionMetradoResumenResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionMetradoResumenCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionMetradoResumenResponse> Handle(CreateCotizacionMetradoResumenCommand request, CancellationToken cancellationToken)
        {
            var resumen = new CotizacionMetradoResumen
            {
                NegocioCode = request.NegocioCode.Trim().ToUpperInvariant(),
                Year = request.Year,
                Month = request.Month,
                Code = request.Code.Trim().ToUpperInvariant(),
                LevelNumber = request.LevelNumber,

                LevelName = request.LevelName.Trim(),
                AlturaLosaCode = request.AlturaLosaCode.Trim().ToUpperInvariant(),
                OverloadOrShortage = request.OverloadOrShortage.Trim(),

                LinealMeters = request.LinealMeters,
                TotalVaults = request.TotalVaults,
                TotalMeters = request.TotalMeters,
                TotalPrice = request.TotalPrice,
                PricePerM2 = request.PricePerM2,
                Quantity = request.Quantity,
                BuildingLevel = request.BuildingLevel.Trim(),
                HasTransport = request.HasTransport,

                TotalVaultsAlt = request.TotalVaultsAlt,
                TotalMetersAlt = request.TotalMetersAlt,
                TotalPriceAlt = request.TotalPriceAlt,
                PricePerM2Alt = request.PricePerM2Alt,

                HasMinPrice = request.HasMinPrice,
                MinTotal = request.MinTotal,
                MinTotalAlt = request.MinTotalAlt,
                MinTotalB = request.MinTotalB,
                MinTotalAltB = request.MinTotalAltB,
                HasTransportB = request.HasTransportB,
                HasMinPriceB = request.HasMinPriceB
            };

            await _uow.Facturacion.Transacciones.CotizacionesMetradoResumen.AddAsync(resumen, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(resumen);
        }

        internal static CotizacionMetradoResumenResponse Map(CotizacionMetradoResumen r) => new()
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
        };
    }
}
