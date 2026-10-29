// Cepheus.Domain/Logistica/Enum/EstadoCotizacion.cs
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado de una Cotización (RFQ). Legacy: dbo.RCotizaciones.Estado_cot
    /// (01 Pendiente, 04 Anulado, 11 Cerrado). Se agrega EnEvaluacion
    /// (confirmado) para el momento en que ya llegaron/vencieron las
    /// respuestas de los proveedores y el comprador está comparando antes
    /// de cerrar.
    ///
    /// Transiciones (tentativas, se afinan al construir ChangeEstado):
    ///   Pendiente -> EnEvaluacion | Anulado
    ///   EnEvaluacion -> Cerrado | Anulado
    ///   Cerrado -> (final)
    ///   Anulado -> (final)
    ///
    /// Cerrado implica que ya se seleccionó proveedor(es) — siguiente paso
    /// es generar la Orden de Compra (módulo futuro).
    /// </summary>
    public enum EstadoCotizacion
    {
        Pendiente = 1,
        EnEvaluacion = 20,
        Cerrado = 11,
        Anulado = 4
    }
}