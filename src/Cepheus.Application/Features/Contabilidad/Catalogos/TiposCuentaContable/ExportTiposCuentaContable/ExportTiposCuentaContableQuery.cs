using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposCuentaContable.ExportTiposCuentaContable
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposCuentaContableQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
