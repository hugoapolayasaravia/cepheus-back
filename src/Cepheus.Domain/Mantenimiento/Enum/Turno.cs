namespace Cepheus.Domain.Mantenimiento.Enum
{
    /// <summary>
    /// Turno de trabajo en el que se ejecuta/registra una Orden de Trabajo.
    ///
    /// Legacy: dbo.MOrdenTrabajos.Turno_Otr (char(1), default 'M', valor
    /// visto en el dato de muestra). Catálogo cerrado y fijo -> enum, mismo
    /// criterio que EstadoOrdenTrabajo.
    ///
    ///   'M' -> Manana (default, igual que el legacy)
    ///   'T' -> Tarde
    ///   'N' -> Noche
    /// </summary>
    public enum Turno
    {
        Manana = 0,
        Tarde = 1,
        Noche = 2
    }
}
