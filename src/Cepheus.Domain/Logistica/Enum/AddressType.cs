namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Tipo de dirección de un proveedor. Legacy: MProveedorDirecciones.TipoDireccion
    /// char(2). Valores asumidos por falta de especificación explícita — confirmar
    /// con el negocio si hay más tipos.
    /// </summary>
    public enum AddressType
    {
        Fiscal,
        Entrega,
        Correspondencia
    }
}
