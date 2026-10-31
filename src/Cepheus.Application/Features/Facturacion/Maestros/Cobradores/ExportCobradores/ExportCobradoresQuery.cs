using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Cobradores.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Cobradores.ExportCobradores
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportCobradoresQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
