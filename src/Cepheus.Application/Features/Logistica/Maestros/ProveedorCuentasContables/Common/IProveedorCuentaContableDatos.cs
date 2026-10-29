namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentasContables.Common
{
    public interface IProveedorCuentaContableDatos
    {
        string ProveedorCode { get; }
        string MonedaCode { get; }
        string AccountingAccountCode { get; }
    }
}
