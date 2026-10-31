using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.ExportActividadesMantenimiento
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportActividadesMantenimientoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? ActividadCode { get; set; }
    }
}
