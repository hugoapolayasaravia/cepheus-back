namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Origen de una línea de caja chica. Legacy: dbo.MCajaChicaDetalle.Opcion_Ing (char(1)):
    ///   I -> Manual (ingreso manual del documento)
    ///   N -> NotaIngreso (documento tomado de una Nota de Ingreso de Logística)
    ///   S -> OrdenServicio (documento tomado de una Orden de Servicio de Logística)
    /// </summary>
    public enum OrigenLineaCajaChica
    {
        Manual = 1,
        NotaIngreso = 2,
        OrdenServicio = 3
    }
}
