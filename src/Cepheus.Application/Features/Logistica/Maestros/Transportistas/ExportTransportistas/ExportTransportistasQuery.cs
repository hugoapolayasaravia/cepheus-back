using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.ExportTransportistas
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTransportistasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public bool? IsOwnFleet { get; set; }
    }
}
