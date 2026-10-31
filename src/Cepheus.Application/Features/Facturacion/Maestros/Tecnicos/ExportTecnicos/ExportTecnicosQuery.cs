using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.ExportTecnicos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTecnicosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
