using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosSolicitudNotaCredito.ExportMotivosSolicitudNotaCredito
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportMotivosSolicitudNotaCreditoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? ParentCode { get; set; }
        public bool? OnlyCurrent { get; set; }
    }
}
