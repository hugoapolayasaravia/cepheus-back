using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cepheus.Infrastructure.Persistence.Converters
{
    /// <summary>
    /// Guarda un enum con el código de texto del legacy (p. ej. "N", "S", "P") en lugar de su valor numérico,
    /// para que los datos migrados del PowerBuilder se carguen sin traducir.
    /// </summary>
    public sealed class LegacyCharConverter<TEnum> : ValueConverter<TEnum, string>
        where TEnum : struct, System.Enum
    {
        public LegacyCharConverter(params (TEnum Value, string Code)[] map)
            : base(
                v => ToCode(v, map),
                s => FromCode(s, map))
        {
        }

        private static string ToCode(TEnum value, (TEnum Value, string Code)[] map)
        {
            foreach (var (v, c) in map)
            {
                if (EqualityComparer<TEnum>.Default.Equals(v, value))
                {
                    return c;
                }
            }

            throw new InvalidOperationException($"El valor {value} de {typeof(TEnum).Name} no tiene código legacy.");
        }

        private static TEnum FromCode(string code, (TEnum Value, string Code)[] map)
        {
            var trimmed = code?.Trim();

            foreach (var (v, c) in map)
            {
                if (string.Equals(c, trimmed, StringComparison.OrdinalIgnoreCase))
                {
                    return v;
                }
            }

            throw new InvalidOperationException($"El código legacy '{code}' no existe en {typeof(TEnum).Name}.");
        }
    }
}
