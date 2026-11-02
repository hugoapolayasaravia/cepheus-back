using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetraVentaReport;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetrasVentaReportLote
{
    public class GetLetrasVentaReportLoteQueryHandler
        : IRequestHandler<GetLetrasVentaReportLoteQuery, List<LetraVentaReportResponse>>
    {
        private readonly ISender _sender;

        public GetLetrasVentaReportLoteQueryHandler(ISender sender)
        {
            _sender = sender;
        }

        public async Task<List<LetraVentaReportResponse>> Handle(
            GetLetrasVentaReportLoteQuery request, CancellationToken cancellationToken)
        {
            if (request.Letras is null || request.Letras.Count == 0)
                throw new InvalidOperationException("Seleccione al menos una letra para imprimir.");

            var result = new List<LetraVentaReportResponse>(request.Letras.Count);
            foreach (var key in request.Letras)
            {
                result.Add(await _sender.Send(new GetLetraVentaReportQuery(key.PlantaCode, key.Number, "001", "S", AvalOptional: true), cancellationToken));
            }
            return result;
        }
    }
}
