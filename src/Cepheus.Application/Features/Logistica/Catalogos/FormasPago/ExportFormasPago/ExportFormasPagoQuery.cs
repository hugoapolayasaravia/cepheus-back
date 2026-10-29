using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.ExportFormasPago
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportFormasPagoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public bool? IsCredit { get; set; }
    }
}
