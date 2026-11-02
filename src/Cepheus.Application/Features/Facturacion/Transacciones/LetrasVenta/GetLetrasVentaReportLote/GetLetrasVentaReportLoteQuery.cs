using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetraVentaReport;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetrasVentaReportLote
{
    public class LetraVentaKeyInput
    {
        public string PlantaCode { get; set; } = default!;
        public string Number { get; set; } = default!;
    }

    /// <summary>
    /// Impresión de letras por lote (legacy w_cre_rpt_x_lote_letras): devuelve los datos de impresión de cada letra
    /// marcada (legacy cr_Letras_x_Lotes / dw_rpt_letras_l; papel 210x335 mm). Toma el aval 001 del cliente si lo tiene,
    /// como el legacy, sin preguntar por aval. La búsqueda previa de las letras se hace con el listado paginado (mismos filtros del legacy).
    /// </summary>
    public record GetLetrasVentaReportLoteQuery(List<LetraVentaKeyInput> Letras)
        : IRequest<List<LetraVentaReportResponse>>;
}
