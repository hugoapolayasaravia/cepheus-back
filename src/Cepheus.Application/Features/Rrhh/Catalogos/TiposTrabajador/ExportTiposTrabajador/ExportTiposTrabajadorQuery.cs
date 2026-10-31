using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.ExportTiposTrabajador
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposTrabajadorQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
