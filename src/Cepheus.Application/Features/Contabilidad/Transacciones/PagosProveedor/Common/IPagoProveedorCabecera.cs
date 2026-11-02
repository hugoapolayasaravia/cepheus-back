namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common
{
    /// <summary>Campos de cabecera comunes a crear y editar un Pago a Proveedor.</summary>
    public interface IPagoProveedorCabecera
    {
        string ProveedorCode { get; }
        DateTime FechaEmision { get; }
        string BancoCode { get; }
        string CuentaBancariaNumero { get; }
        string? NumeroCheque { get; }
        string NumeroControl { get; }
        string PlantaCode { get; }
        string? PagarA { get; }
        string? Glosa { get; }
        string FormaPagoCancelacionCode { get; }
        int? ProveedorCuentaId { get; }
        bool Canje { get; }
        bool DepositoElectronico { get; }
    }
}
