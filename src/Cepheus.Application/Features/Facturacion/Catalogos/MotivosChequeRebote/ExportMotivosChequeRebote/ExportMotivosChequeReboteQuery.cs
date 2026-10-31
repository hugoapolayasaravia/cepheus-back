using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.ExportMotivosChequeRebote
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportMotivosChequeReboteQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
