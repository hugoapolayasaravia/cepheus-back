using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.ExportTiposCliente
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposClienteQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
