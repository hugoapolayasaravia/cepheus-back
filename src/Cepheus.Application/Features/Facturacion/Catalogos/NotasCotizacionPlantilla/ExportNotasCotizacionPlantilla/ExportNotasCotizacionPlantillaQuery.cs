using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.ExportNotasCotizacionPlantilla
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportNotasCotizacionPlantillaQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? NegocioCode { get; set; }
        public OpcionNotaCotizacion? Option { get; set; }
    }
}
