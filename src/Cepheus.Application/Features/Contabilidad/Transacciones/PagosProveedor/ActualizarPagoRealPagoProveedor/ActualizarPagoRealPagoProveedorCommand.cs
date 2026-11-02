using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.ActualizarPagoRealPagoProveedor
{
    /// <summary>Legacy w_com_pagoreal_cheques: monto realmente pagado (PagSolReal_che).</summary>
    public record ActualizarPagoRealPagoProveedorCommand(string Code, decimal PagoRealSol)
        : IRequest<PagoProveedorResponse>;
}
