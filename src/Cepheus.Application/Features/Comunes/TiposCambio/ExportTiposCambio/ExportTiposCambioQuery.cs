using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposCambio.ExportTiposCambio
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposCambioQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? MonedaCode { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
