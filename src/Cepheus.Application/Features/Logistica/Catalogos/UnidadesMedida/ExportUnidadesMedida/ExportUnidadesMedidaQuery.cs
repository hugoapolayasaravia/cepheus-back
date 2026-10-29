using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.ExportUnidadesMedida
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportUnidadesMedidaQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
