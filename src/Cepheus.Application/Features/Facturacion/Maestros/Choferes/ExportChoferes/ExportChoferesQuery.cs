using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.ExportChoferes
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportChoferesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? TransportistaCode { get; set; }
    }
}
