// Cepheus.Domain/Logistica/Enum/EstadoGuia.cs
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado de una Guía de Remisión. Legacy: dbo.MGuias.Codigo_Est
    /// (comentario del DDL: "01 Pendiente | 04 Anulado").
    ///
    /// Mapeo de valores legacy:
    ///   01 Pendiente -> Pendiente
    ///   04 Anulado   -> Anulado
    ///
    /// El código PowerBuilder (w_guias_logistica) también valida los códigos
    /// 09 (Aprobado) y 11 (Cerrado), pero el DDL define solo 01 y 04 y las
    /// guías no tienen flujo de aprobación/cierre en el nuevo modelo, por lo
    /// que esos valores NO se migran.
    ///
    /// Transiciones válidas:
    ///   Pendiente -> Anulado
    ///   Anulado   -> (ninguna, estado final)
    ///
    /// Toda Guía nueva nace en Pendiente.
    /// </summary>
    public enum EstadoGuia
    {
        Pendiente = 1,
        Anulado = 4
    }
}
