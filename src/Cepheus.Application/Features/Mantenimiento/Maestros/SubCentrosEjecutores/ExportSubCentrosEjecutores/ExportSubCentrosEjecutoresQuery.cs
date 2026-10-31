using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.ExportSubCentrosEjecutores
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportSubCentrosEjecutoresQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? CentroEjecutorCode { get; set; }
    }
}
