using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Plantas.ExportPlantas
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportPlantasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public bool? IsProductionPlant { get; set; }
    }
}
