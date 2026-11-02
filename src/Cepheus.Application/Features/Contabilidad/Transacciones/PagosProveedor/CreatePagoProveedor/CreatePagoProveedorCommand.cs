using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.CreatePagoProveedor
{
    public record CreatePagoProveedorCommand(
        string ProveedorCode,
        DateTime FechaEmision,
        string BancoCode,
        string CuentaBancariaNumero,
        string? NumeroCheque,
        string NumeroControl,
        string PlantaCode,
        string? PagarA,
        string? Glosa,
        string FormaPagoCancelacionCode,
        int? ProveedorCuentaId,
        bool Canje = false,
        bool DepositoElectronico = false
    ) : IRequest<PagoProveedorResponse>, IPagoProveedorCabecera;
}
