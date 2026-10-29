using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.GetCotizacionNotasByCotizacion
{
    public class GetCotizacionNotasByCotizacionQueryHandler
        : IRequestHandler<GetCotizacionNotasByCotizacionQuery, List<CotizacionNotaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCotizacionNotasByCotizacionQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<CotizacionNotaResponse>> Handle(
            GetCotizacionNotasByCotizacionQuery request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var notas = await _uow.Facturacion.Transacciones.CotizacionesNotas.Query()
                .AsNoTracking()
                .Where(n => n.NegocioCode == negocio && n.Year == request.Year
                         && n.Month == request.Month && n.Code == code)
                .OrderBy(n => n.Sequence)
                .ToListAsync(cancellationToken);

            return notas.Select(n => new CotizacionNotaResponse
            {
                NegocioCode = n.NegocioCode,
                Year = n.Year,
                Month = n.Month,
                Code = n.Code,
                Sequence = n.Sequence,
                Description = n.Description,
                Option = n.Option,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                RowVersion = n.RowVersion
            }).ToList();
        }
    }
}
