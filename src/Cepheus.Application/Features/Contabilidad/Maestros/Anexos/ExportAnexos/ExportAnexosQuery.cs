using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.Anexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.ExportAnexos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportAnexosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? TipoAnexoCode { get; set; }
    }
}
