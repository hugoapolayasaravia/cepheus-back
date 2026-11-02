using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.ExportCentrosCosto
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportCentrosCostoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? PlantaCode { get; set; }
    }
}
