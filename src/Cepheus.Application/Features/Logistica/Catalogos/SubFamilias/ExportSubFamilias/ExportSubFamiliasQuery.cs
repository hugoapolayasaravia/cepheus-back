using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.ExportSubFamilias
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportSubFamiliasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? FamiliaCode { get; set; }
    }
}
