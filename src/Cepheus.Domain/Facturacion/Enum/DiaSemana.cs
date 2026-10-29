namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Día de la semana, con la numeración propia del legacy (Lunes=0 .. Domingo=6),
    /// DISTINTA de System.DayOfWeek (.NET usa Domingo=0 .. Sábado=6). Se define
    /// un enum propio para no confundir ambas convenciones y para poder migrar
    /// los valores numéricos existentes en dbo.MObras.NUM_DIAS_MES tal cual.
    ///
    /// Legacy: MObras.NUM_DIAS_MES int NULL. A pesar del nombre de la columna
    /// ("número de días del mes"), el comentario del script original la
    /// documenta como una tabla de correspondencia día-de-semana -> número:
    ///   Lunes=0, Martes=1, Miercoles=2, Jueves=3, Viernes=4, Sabado=5, Domingo=6
    /// El propósito de negocio exacto (¿día de reparto? ¿día de visita?) no
    /// está documentado en el script — se preserva el valor y su nombre
    /// original (ScheduledWeekday en Obra) hasta confirmar el uso real.
    /// </summary>
    public enum DiaSemana
    {
        Lunes = 0,
        Martes = 1,
        Miercoles = 2,
        Jueves = 3,
        Viernes = 4,
        Sabado = 5,
        Domingo = 6
    }
}
