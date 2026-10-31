using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.ExportTrabajadores
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTrabajadoresQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
