using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.ExportClasificacionesCliente
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportClasificacionesClienteQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
