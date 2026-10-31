using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.ExportResponsablesObra
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportResponsablesObraQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? ClienteCode { get; set; }
        public string? ObraCode { get; set; }
    }
}
