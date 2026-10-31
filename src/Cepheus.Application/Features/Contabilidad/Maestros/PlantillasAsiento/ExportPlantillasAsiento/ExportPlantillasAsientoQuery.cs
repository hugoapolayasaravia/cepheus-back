using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.ExportPlantillasAsiento
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportPlantillasAsientoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public ProcesoAsiento? Process { get; set; }
        public string? TipoAsientoCode { get; set; }
    }
}
