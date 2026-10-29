namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Tipo de la OT de Ventas (PedidoVentaResumen.Tipo_ped). Se guarda con la letra legacy.
    ///   N Normal | S Saldo | A Anticipo | D Descuento (descuenta un anticipo) | C Consolidación.
    /// Saldo y Consolidación las genera la fase de programación; no se crean a mano.
    /// </summary>
    public enum TipoOrdenTrabajoVenta
    {
        Normal = 1,
        Saldo = 2,
        Anticipo = 3,
        Descuento = 4,
        Consolidacion = 5,
    }
}
