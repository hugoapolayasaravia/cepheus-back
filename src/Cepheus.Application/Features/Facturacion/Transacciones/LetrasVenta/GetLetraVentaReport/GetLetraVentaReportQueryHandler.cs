using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetraVentaReport
{
    public class GetLetraVentaReportQueryHandler : IRequestHandler<GetLetraVentaReportQuery, LetraVentaReportResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetLetraVentaReportQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LetraVentaReportResponse> Handle(GetLetraVentaReportQuery request, CancellationToken cancellationToken)
        {
            var plantaCode = LetraVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            var letra = await _uow.Facturacion.Transacciones.LetrasVenta.Query()
                .AsNoTracking()
                .Include(l => l.Cliente).ThenInclude(c => c.Ubigeo)
                .Include(l => l.Banco)
                .FirstOrDefaultAsync(l => l.PlantaCode == plantaCode && l.Number == number, cancellationToken)
                ?? throw new KeyNotFoundException($"Letra {LetraVentaRules.FormatKey(plantaCode, number)} no encontrada.");

            var documents = await _uow.Facturacion.Transacciones.CancelacionesVentaDetalle.Query()
                .AsNoTracking()
                .Where(d => d.CancelacionCode == letra.CancelacionCode)
                .OrderBy(d => d.Item)
                .Select(d => new LetraVentaReportDocument
                {
                    TypeCode = d.TypeCode,
                    DocumentNumber = d.DocumentNumber,
                    DocumentDate = d.DocumentDate,
                    PaidAmount = d.PaidAmount,
                    DocumentCurrencyCode = d.DocumentCurrencyCode
                })
                .ToListAsync(cancellationToken);

            LetraVentaReportAval? aval = null;
            if (!string.IsNullOrWhiteSpace(request.AvalSequence))
            {
                var seq = request.AvalSequence.Trim();
                var a = await _uow.Facturacion.Maestros.ClientesResponsablesLetra.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ClienteCode == letra.ClienteCode && x.Sequence == seq, cancellationToken);
                if (a is null && !request.AvalOptional)
                    throw new KeyNotFoundException($"El cliente {letra.ClienteCode} no tiene el aval {seq}.");
                if (a is not null)
                {
                    aval = new LetraVentaReportAval
                    {
                        Sequence = a.Sequence,
                        FullName = string.Join(" ", new[] { a.FirstNames, a.PaternalSurname, a.MaternalSurname }
                            .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!.Trim())),
                        DocumentTypeCode = a.DocumentTypeCode,
                        DocumentNumber = a.DocumentNumber,
                        Address = a.Address,
                        Phone = a.Phone
                    };
                }
            }

            var rate = letra.ExchangeRate;
            var isSoles = letra.MonedaCode == "PEN";
            var totalSoles = isSoles ? letra.TotalAmount : Math.Round(letra.TotalAmount * rate, 2);
            var totalDollars = !isSoles ? letra.TotalAmount : (rate > 0 ? Math.Round(letra.TotalAmount / rate, 2) : 0m);

            return new LetraVentaReportResponse
            {
                PlantaCode = letra.PlantaCode,
                Number = letra.Number,
                IssueDate = letra.IssueDate,
                DueDate = letra.DueDate,
                ClienteCode = letra.ClienteCode,
                ClienteName = letra.Cliente.Name,
                ClienteDocumentTypeCode = letra.Cliente.DocumentTypeCode,
                ClienteDocument = letra.Cliente.DocumentNumber,
                ClienteAddress = letra.Cliente.Address,
                ClienteUbigeo = letra.Cliente.Ubigeo?.FullAddress,
                ClienteDistrict = letra.Cliente.Ubigeo?.District,
                ClientePhone = letra.Cliente.Phone,
                BancoName = letra.Banco?.Name,
                MonedaCode = letra.MonedaCode,
                TotalAmount = letra.TotalAmount,
                ExchangeRate = rate,
                TotalSoles = totalSoles,
                TotalDollars = totalDollars,
                Interest = letra.Interest,
                Description = letra.Description,
                AmountInWords = MontoEnLetras.ToWords(letra.TotalAmount, letra.MonedaCode),
                AvalOption = request.AvalOption == "N" ? "N" : "S",
                Aval = aval,
                AcceptedByClient = letra.AcceptedByClient,
                Status = letra.Status.ToString(),
                Documents = documents
            };
        }
    }
}
