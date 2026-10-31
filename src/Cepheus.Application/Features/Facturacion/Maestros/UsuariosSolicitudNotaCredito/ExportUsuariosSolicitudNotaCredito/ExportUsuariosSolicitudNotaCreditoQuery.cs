using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.UsuariosSolicitudNotaCredito.ExportUsuariosSolicitudNotaCredito
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportUsuariosSolicitudNotaCreditoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? NegocioCode { get; set; }
        public string? Stage { get; set; }
        public int? UserId { get; set; }
        public bool? IsPrincipal { get; set; }
    }
}
