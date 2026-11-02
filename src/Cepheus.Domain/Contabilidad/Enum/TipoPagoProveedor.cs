namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Tipo de operación de la cancelación. Legacy: dbo.RCheCompras.tipo_che (char(1)).
    /// Los valores salen del proceso (cada ventana fija el suyo):
    ///   1 Proveedores (w_com_reg_cheques), 2 Clientes (w_com_reg_cheques_clientes),
    ///   3 Mixtos (cliente-proveedor).
    /// El valor numérico coincide con el código legacy.
    /// </summary>
    public enum TipoPagoProveedor
    {
        Proveedores = 1,
        Clientes = 2,
        Mixtos = 3
    }
}
