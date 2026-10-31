using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.ExportAlturasLosa
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportAlturasLosaQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
