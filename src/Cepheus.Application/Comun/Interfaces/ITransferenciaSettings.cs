namespace Cepheus.Application.Comun.Interfaces;

/// <summary>
/// Parámetros de las transferencias entre almacenes. En el legacy estaban fijos en el código del PowerBuilder.
/// </summary>
public interface ITransferenciaSettings
{
    /// <summary>
    /// Código del proveedor con el que se registran los ingresos por transferencia (la propia empresa; en el legacy
    /// era '08532'). Clave de configuración: Logistica:ProveedorTransferenciasCode.
    /// </summary>
    string ProveedorCode { get; }
}
