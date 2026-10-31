using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.ExportObras
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportObrasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? ClienteCode { get; set; }
        public string? VendedorCode { get; set; }
        public string? CobradorCode { get; set; }
        public string? Estado { get; set; }
    }
}
