// Cepheus.Domain/Logistica/Enum/EstadoGuiaDetalle.cs
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado de una línea de Guía de Remisión (GuiaDetalle). Legacy:
    /// dbo.MGuiasDet.Codigo_Est.
    ///
    /// Mapeo de valores legacy:
    ///   01 Pendiente -> Pendiente
    ///   04 Anulado   -> Anulado
    ///
    /// Toda línea nueva nace en Pendiente. Al anular la Guía, el sistema
    /// anula también todas sus líneas (mismo comportamiento que el legacy).
    /// </summary>
    public enum EstadoGuiaDetalle
    {
        Pendiente = 1,
        Anulado = 4
    }
}
