using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.GetNotaDebitoVentaReport
{
    public class GetNotaDebitoVentaReportQueryHandler
        : IRequestHandler<GetNotaDebitoVentaReportQuery, NotaDebitoVentaReportResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNotaDebitoVentaReportQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaDebitoVentaReportResponse> Handle(
            GetNotaDebitoVentaReportQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaDebitoVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            var nota = await _uow.Facturacion.Transacciones.NotasDebitoVenta.Query()
                .AsNoTracking()
                .Include(n => n.Cliente).ThenInclude(c => c.Ubigeo)
                .Include(n => n.DeliveryUbigeo)
                .Include(n => n.Details).ThenInclude(d => d.Producto).ThenInclude(p => p.Unit)
                .FirstOrDefaultAsync(n => n.PlantaCode == plantaCode && n.Number == number, cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException(
                    $"Nota de débito {NotaDebitoVentaRules.FormatKey(plantaCode, number)} no encontrada.");
            }

            var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(_uow, nota.IssueDate, cancellationToken);

            return new NotaDebitoVentaReportResponse
            {
                PlantaCode = nota.PlantaCode,
                Number = nota.Number,
                IssueDate = nota.IssueDate,
                ClienteCode = nota.ClienteCode,
                ClienteName = nota.Cliente.Name,
                ClienteDocument = nota.Cliente.DocumentNumber,
                ClienteAddress = nota.Cliente.Address,
                ClienteUbigeo = nota.Cliente.Ubigeo?.FullAddress,
                DeliveryAddress = nota.DeliveryAddress,
                DeliveryUbigeo = nota.DeliveryUbigeo?.FullAddress,
                DocumentLabel = nota.DocumentTypeCode switch
                {
                    "01" => "FAC",
                    "03" => "BOL",
                    _ => null
                },
                DocumentNumber = nota.DocumentNumber,
                MonedaCode = nota.MonedaCode,
                SubTotal = nota.SubTotal,
                IgvAmount = nota.IgvAmount,
                TotalAmount = nota.TotalAmount,
                IgvPercentage = igvPercentage,
                Description = nota.Description,
                StatusCode = (int)nota.Status,
                Status = nota.Status.ToString(),
                Lines = nota.Details
                    .OrderBy(d => d.Line)
                    .Select(d => new NotaDebitoVentaReportLine
                    {
                        Description = d.Description,
                        Quantity = d.Quantity,
                        Unit = d.Producto?.Unit?.Name,
                        UnitPrice = d.UnitPrice,
                        Total = d.Total
                    }).ToList()
            };
        }
    }
}
