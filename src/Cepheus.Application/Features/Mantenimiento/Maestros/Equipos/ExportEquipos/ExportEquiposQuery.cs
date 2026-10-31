using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.ExportEquipos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportEquiposQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? SubCentroCostoCode { get; set; }
        public int? Nivel { get; set; }
    }
}
