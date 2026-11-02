using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.MarcarImpresionPagoProveedor
{
    /// <summary>
    /// Legacy w_com_rep_cheques: registra que se imprimió un comprobante del pago.
    /// Tipo: "Retencion" (legacy '0') o "CajaBancos" (legacy '1').
    /// </summary>
    public record MarcarImpresionPagoProveedorCommand(string Code, string Tipo) : IRequest<PagoProveedorResponse>;
}
