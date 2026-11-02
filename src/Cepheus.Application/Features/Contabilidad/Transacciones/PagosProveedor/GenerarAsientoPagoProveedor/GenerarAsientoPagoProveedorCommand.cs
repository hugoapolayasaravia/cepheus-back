using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GenerarAsientoPagoProveedor
{
    /// <summary>Genera el asiento de cancelación del pago (y el comprobante de retención si hay retención).</summary>
    public record GenerarAsientoPagoProveedorCommand(string Code) : IRequest<PagoProveedorResponse>;
}
