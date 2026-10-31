using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.ExportFletes
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportFletesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public bool? IsDefault { get; set; }
    }
}
