using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.ExportTransportistas
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTransportistasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
