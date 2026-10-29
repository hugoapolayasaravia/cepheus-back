namespace Cepheus.Domain.Mantenimiento.Enum
{
    /// <summary>
    /// Estado operativo de una unidad/equipo en un día dado (UnidadParada,
    /// UnidadHorometro). Reemplaza a la columna legacy Estado_Equ char(2)
    /// (códigos de TEstados) — catálogo cerrado, se modela como enum igual
    /// que EstadoOrdenTrabajo.
    ///
    /// Mapeo de valores legacy:
    ///   '22' Operativo     -> Operativo
    ///   '23' Inoperativo   -> Inoperativo
    ///   (código no confirmado) Mantenimiento -> Mantenimiento
    ///
    /// PENDIENTE al migrar datos: confirmar con
    /// SELECT DISTINCT Estado_Equ FROM MUnidad_Parada el código legacy de
    /// "Mantenimiento" y cualquier otro valor existente.
    /// </summary>
    public enum EstadoEquipo
    {
        Operativo = 0,
        Inoperativo = 1,
        Mantenimiento = 2
    }
}
