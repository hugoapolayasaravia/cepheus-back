namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la Hoja de Corte. Reemplaza a dbo.TEstados del legacy (catálogo
    /// cerrado de estados fijos, sin atributos propios más allá de código y
    /// descripción) — mismo criterio que EstadoCotizacion. El valor numérico del
    /// enum es el código legacy (Codigo_Est), igual que EstadoNotaIngreso.
    ///
    /// Valores usados por dbo.RHojaCorte.codigo_est (DDL: "01 Pendiente | 04 Anulado
    /// | Procesado"; el PB usa '13' para Procesado):
    ///   01 -> Pendiente (toda Hoja de Corte nace Pendiente, también la copia)
    ///   04 -> Anulado
    ///   13 -> Procesado (la toma la Orden de Trabajo de Ventas)
    ///
    /// Independiente de los enums de otros módulos (Logística/Mantenimiento).
    /// </summary>
    public enum EstadoHojaCorte
    {
        Pendiente = 1,
        Anulado = 4,
        Procesado = 13
    }
}
