using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Comunes.PeriodosIgv.ExportPeriodosIgv
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportPeriodosIgvQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
