namespace Cepheus.Domain.Mantenimiento.Enum
{
    /// <summary>
    /// Estado de una Orden de Trabajo. Reemplaza a dbo.TEstados del legacy
    /// (catálogo cerrado de 5 valores fijos que controla el flujo de la OT,
    /// sin atributos propios más allá de código+descripción) — se modela
    /// como enum en vez de tabla, igual criterio que AbcClass o ProviderType
    /// en Logística.
    ///
    /// Mapeo de valores legacy (dbo.TEstados):
    ///   PENDIENTE                    -> Pendiente
    ///   Espera Aprobar                -> EsperaAprobacion
    ///   APROBADA                      -> Aprobada
    ///   EN PROCESO DE EJECUCION       -> EnProceso
    ///   CONLUIDA O TERMINADA (typo)   -> Concluida
    ///
    /// Toda OrdenTrabajo nueva nace en Pendiente — no se recibe desde el
    /// cliente al crear.
    /// </summary>
    public enum EstadoOrdenTrabajo
    {
        Pendiente = 0,
        EsperaAprobacion = 1,
        Aprobada = 2,
        EnProceso = 3,
        Concluida = 4
    }
}
