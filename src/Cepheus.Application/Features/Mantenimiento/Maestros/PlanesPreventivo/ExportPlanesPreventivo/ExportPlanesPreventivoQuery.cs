using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ExportPlanesPreventivo
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportPlanesPreventivoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? PlantaCode { get; set; }
        public string? Code { get; set; }
        public string? EquipoCode { get; set; }
        public string? ResponsableCode { get; set; }
    }
}
