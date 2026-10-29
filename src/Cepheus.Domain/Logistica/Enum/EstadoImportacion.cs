// Cepheus.Domain/Logistica/Enum/EstadoImportacion.cs
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado de una Importación. Legacy: dbo.MImportacionRes.Codigo_Est y
    /// dbo.MImportacionDet.Codigo_Est (01 Pendiente, 13 Procesado, 04 Anulado).
    ///
    /// Los valores numéricos son los códigos legacy (01, 13, 04), igual que EstadoOrdenCompra y EstadoNotaIngreso.
    ///
    /// El mismo enum se usa en cabecera y detalle: en el legacy el detalle
    /// (por artículo/proveedor) pasa a Procesado cuando se genera su ingreso
    /// a stock, y la cabecera pasa a Procesado recién cuando NINGÚN detalle
    /// queda Pendiente (generación parcial por proveedor).
    ///
    /// Transiciones:
    ///   Pendiente -> Procesado | Anulado
    ///   Procesado -> Anulado   (solo con reversión de stock; ver Generar/Anular)
    ///   Anulado   -> (final)
    ///
    /// El estado Procesado NO se cambia manualmente: lo establece la
    /// generación de ingreso a stock.
    /// </summary>
    public enum EstadoImportacion
    {
        Pendiente = 1,
        Procesado = 13,
        Anulado = 4
    }
}
