using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GetReporteDetraccion
{
    public class GetReporteDetraccionQueryHandler : IRequestHandler<GetReporteDetraccionQuery, ReporteDetraccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetReporteDetraccionQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ReporteDetraccionResponse> Handle(
            GetReporteDetraccionQuery request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);

            var header = await _uow.Contabilidad.Transacciones.Detracciones.Query().AsNoTracking()
                .Where(d => d.Year == year && d.Period == period && d.LotNumber == lot)
                .Select(d => new { d.StartDate, d.EndDate, d.Status })
                .FirstOrDefaultAsync(cancellationToken);

            if (header is null)
            {
                throw new KeyNotFoundException(DetraccionRules.NotFoundMessage(year, period, lot));
            }

            var rows = await _uow.Contabilidad.Transacciones.DetraccionDocumentos.Query().AsNoTracking()
                .Where(d => d.Year == year && d.Period == period && d.LotNumber == lot)
                .OrderBy(d => d.ItemNumber).ThenBy(d => d.ComprobantePagoCode).ThenBy(d => d.NumeroDocumento)
                .Select(d => new ReporteDetraccionRow
                {
                    ItemNumber = d.ItemNumber,
                    AccountNumber = d.Item.AccountNumber,
                    ProveedorRuc = d.Item.ProveedorRuc,
                    ProveedorName = d.Item.Proveedor.LegalName,
                    TipoOperacionCode = d.Item.TipoOperacionCode,
                    TipoOperacionName = d.Item.TipoOperacion.Name,
                    TipoBienCode = d.Item.TipoBienCode,
                    TipoBienName = d.Item.TipoBien.Name,
                    DetractionRate = d.Item.TipoBien.DetractionRate,
                    ComprobantePagoCode = d.ComprobantePagoCode,
                    NumeroDocumento = d.NumeroDocumento,
                    ProcessDate = d.DocumentoCompra.CreatedAt,
                    TotalSol = d.DocumentoCompra.TotalSol,
                    DetraccionSol = d.DetraccionSol,
                    IncludeInExport = d.IncludeInExport
                })
                .ToListAsync(cancellationToken);

            foreach (var row in rows)
            {
                row.NumeroDocumento = row.NumeroDocumento.Trim();
            }

            return new ReporteDetraccionResponse
            {
                Year = year,
                Period = period,
                LotNumber = lot,
                Status = header.Status.ToString(),
                StartDate = header.StartDate,
                EndDate = header.EndDate,
                Rows = rows,
                TotalDetraccionSol = rows.Sum(r => r.DetraccionSol),
                TotalDepositoSol = rows.Where(r => r.IncludeInExport).Sum(r => r.DetraccionSol)
            };
        }
    }
}
