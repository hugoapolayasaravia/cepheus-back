using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.SubCentrosCosto.ExportSubCentrosCosto
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportSubCentrosCostoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? CentroCostoCode { get; set; }
        public string? PlantaCode { get; set; }
        /// <summary>Cadena vacía ("") filtra los subcentros raíz (sin padre).</summary>
        public string? ParentCode { get; set; }
    }
}
