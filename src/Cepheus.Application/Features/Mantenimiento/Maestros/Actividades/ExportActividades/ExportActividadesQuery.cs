using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.ExportActividades
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportActividadesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? VerboActividadCode { get; set; }
        public string? ObjetoActividadCode { get; set; }
    }
}
