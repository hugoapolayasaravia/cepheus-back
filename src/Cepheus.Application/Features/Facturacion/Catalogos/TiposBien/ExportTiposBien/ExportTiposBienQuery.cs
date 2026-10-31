using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.ExportTiposBien
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposBienQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
