using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.AddDocumentosCancelacionVenta
{
    /// <summary>
    /// Agrega documentos a una cancelación Pendiente y sin asiento (legacy: "jalar documentos" y "por documento").
    /// El importe de cada documento va en la moneda de la cancelación; sin importe se aplica todo el saldo.
    /// </summary>
    public class AddDocumentosCancelacionVentaCommand : IRequest<CancelacionVentaResponse>
    {
        public string Code { get; set; } = default!;
        public List<CancelacionVentaDocumentoInput> Documents { get; set; } = new();
        public byte[] RowVersion { get; set; } = default!;
    }
}
