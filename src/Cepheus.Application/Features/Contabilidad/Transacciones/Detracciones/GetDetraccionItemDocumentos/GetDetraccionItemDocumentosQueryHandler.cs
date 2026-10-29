using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetDetraccionItemDocumentos
{
    public class GetDetraccionItemDocumentosQueryHandler
        : IRequestHandler<GetDetraccionItemDocumentosQuery, List<DetraccionDocumentoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetDetraccionItemDocumentosQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<DetraccionDocumentoResponse>> Handle(
            GetDetraccionItemDocumentosQuery request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);
            var proveedor = request.ProveedorCode.Trim();

            var exists = await _uow.Contabilidad.Transacciones.DetraccionItems.Query().AsNoTracking()
                .AnyAsync(i => i.Year == year && i.Period == period && i.LotNumber == lot
                               && i.ProveedorCode == proveedor && i.ItemNumber == request.ItemNumber,
                    cancellationToken);

            if (!exists)
            {
                throw new KeyNotFoundException(
                    $"El ítem {request.ItemNumber} del proveedor {proveedor} no existe en la detracción {DetraccionRules.Label(year, period, lot)}.");
            }

            var rows = await _uow.Contabilidad.Transacciones.DetraccionDocumentos.Query().AsNoTracking()
                .Where(d => d.Year == year && d.Period == period && d.LotNumber == lot
                            && d.ProveedorCode == proveedor && d.ItemNumber == request.ItemNumber)
                .OrderBy(d => d.ComprobantePagoCode).ThenBy(d => d.NumeroDocumento)
                .Select(DetraccionMapper.ToDocumento)
                .ToListAsync(cancellationToken);

            return rows.Select(DetraccionMapper.Clean).ToList();
        }
    }
}
