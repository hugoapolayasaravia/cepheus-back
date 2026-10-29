// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/GetCotizacionByCode/GetCotizacionByCodeQueryHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.GetCotizacionByCode
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
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .AsNoTracking()
                .Include(c => c.Detalles).ThenInclude(d => d.Origenes)
                .Include(c => c.Proveedores).ThenInclude(p => p.Detalles)
                .FirstOrDefaultAsync(c => c.PlantaCode == plantaCode && c.Code == code, cancellationToken);

            if (cotizacion is null)
            {
                throw new KeyNotFoundException($"Cotización {plantaCode}/{code} no encontrada.");
            }

            return CotizacionMapper.Map(cotizacion);
        }
    }
}