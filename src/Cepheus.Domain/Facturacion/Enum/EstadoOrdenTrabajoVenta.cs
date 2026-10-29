namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la OT (PedidoVentaResumen.Codigo_est). El valor numérico es el código legacy,
    /// igual que EstadoHojaCorte: 05 Activo | 06 Inactivo | 11 Cerrado | 42 Cerrado definitivo.
    /// </summary>
    public enum EstadoOrdenTrabajoVenta
    {
        Activo = 5,
        Inactivo = 6,
        Cerrado = 11,
        CerradoDefinitivo = 42,
    }
}
