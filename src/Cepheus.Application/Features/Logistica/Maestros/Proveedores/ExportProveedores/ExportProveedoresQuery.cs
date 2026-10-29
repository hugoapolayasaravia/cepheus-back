using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.ExportProveedores
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportProveedoresQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
