using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.Common;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.UpdateCotizacionMetradoResumen
{
    public class UpdateCotizacionMetradoResumenCommandHandler
        : IRequestHandler<UpdateCotizacionMetradoResumenCommand, CotizacionMetradoResumenResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCotizacionMetradoResumenCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionMetradoResumenResponse> Handle(UpdateCotizacionMetradoResumenCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var current = await _uow.Facturacion.Transacciones.CotizacionesMetradoResumen.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.NegocioCode == negocio && r.Year == request.Year && r.Month == request.Month
                                       && r.Code == code && r.LevelNumber == request.LevelNumber, cancellationToken);

            if (current is null)
                throw new KeyNotFoundException($"Nivel {request.LevelNumber} de la cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrado.");

            var resumen = new CotizacionMetradoResumen
            {
                NegocioCode = negocio,
                Year = request.Year,
                Month = request.Month,
                Code = code,
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
                HasMinPriceB = request.HasMinPriceB,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Facturacion.Transacciones.CotizacionesMetradoResumen.Update(resumen);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El nivel de metrado fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.CreateCotizacionMetradoResumen.CreateCotizacionMetradoResumenCommandHandler.Map(resumen);
        }
    }
}
