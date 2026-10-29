using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Articulos.ExportArticulos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportArticulosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? SubFamiliaCode { get; set; }
        public string? TipoArticuloCode { get; set; }
    }
}
