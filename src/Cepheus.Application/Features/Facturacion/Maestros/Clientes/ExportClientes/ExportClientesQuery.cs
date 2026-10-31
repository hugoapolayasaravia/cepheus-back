using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.ExportClientes
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportClientesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? TipoClienteCode { get; set; }
        public string? ClasificacionClienteCode { get; set; }
        public string? Estado { get; set; }
    }
}
