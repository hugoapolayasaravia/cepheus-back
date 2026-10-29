using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.ExportMotivosDevolucionArticulo
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportMotivosDevolucionArticuloQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public bool? AffectsStock { get; set; }
    }
}
