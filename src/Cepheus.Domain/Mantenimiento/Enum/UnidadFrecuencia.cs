namespace Cepheus.Domain.Mantenimiento.Enum
{
    /// <summary>
    /// Unidad de la frecuencia de un plan de mantenimiento preventivo.
    /// Reemplaza a MManPreventivo.Unidad varchar(3).
    ///
    /// Mapeo de valores legacy (los únicos que contempla
    /// Man_sp_Listado_Calendario_Equipos):
    ///   'DIA' -> Dia | 'SEM' -> Semana | 'MES' -> Mes | 'HOR' -> Hora
    /// </summary>
    public enum UnidadFrecuencia
    {
        Dia = 0,
        Semana = 1,
        Mes = 2,
        Hora = 3
    }
}
