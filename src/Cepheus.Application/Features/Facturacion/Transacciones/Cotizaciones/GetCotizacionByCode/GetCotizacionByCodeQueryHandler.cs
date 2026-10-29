using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.GetCotizacionByCode
{
    public class GetCotizacionByCodeQueryHandler : IRequestHandler<GetCotizacionByCodeQuery, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetCotizacionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(GetCotizacionByCodeQuery request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.NegocioCode == negocio && c.Year == request.Year
                                       && c.Month == request.Month && c.Code == code, cancellationToken);

            if (cotizacion is null)
                throw new KeyNotFoundException($"Cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            return CotizacionMapper.Map(cotizacion);
        }
    }
}
