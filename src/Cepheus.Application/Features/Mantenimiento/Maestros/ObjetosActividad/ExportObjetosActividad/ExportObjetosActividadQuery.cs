using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.ExportObjetosActividad
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportObjetosActividadQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
