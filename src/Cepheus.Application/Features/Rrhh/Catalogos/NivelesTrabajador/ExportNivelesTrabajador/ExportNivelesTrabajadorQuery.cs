using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.ExportNivelesTrabajador
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportNivelesTrabajadorQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
