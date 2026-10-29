using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.LugaresEnvio.ExportLugaresEnvio
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportLugaresEnvioQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
