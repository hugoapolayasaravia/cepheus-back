namespace Cepheus.Application.Comun.Utilidades
{
    /// <summary>
    /// La base guarda las fechas en UTC. Para los archivos exportados se convierten a la hora
    /// de Lima. Para otra zona, cambiar la lista de identificadores de abajo.
    /// </summary>
    public static class HoraLocal
    {
        private static readonly TimeZoneInfo Zona = Resolver();

        private static TimeZoneInfo Resolver()
        {
            foreach (var id in new[] { "America/Lima", "SA Pacific Standard Time" })
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException) { }
                catch (InvalidTimeZoneException) { }
            }

            return TimeZoneInfo.Utc;
        }

        public static DateTime? Convertir(DateTime? utc)
        {
            return utc is null
                ? null
                : TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc.Value, DateTimeKind.Utc), Zona);
        }

        public static DateTime Ahora()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zona);
        }
    }
}
