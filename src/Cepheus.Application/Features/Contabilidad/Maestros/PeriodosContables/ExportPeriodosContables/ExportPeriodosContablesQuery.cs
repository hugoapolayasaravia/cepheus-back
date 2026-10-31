using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.ExportPeriodosContables
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportPeriodosContablesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public EstadoPeriodoContable? Status { get; set; }
    }
}
