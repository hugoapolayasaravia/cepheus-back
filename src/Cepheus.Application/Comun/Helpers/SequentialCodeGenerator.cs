using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Comun.Helpers
{
    /// <summary>
    /// Genera códigos correlativos numéricos, con cero a la izquierda, para
    /// los catálogos de Logística cuya PK es un string natural (sin Id
    /// surrogate). Usado por todos los CreateXCommandHandler del módulo.
    /// </summary>
    public static class SequentialCodeGenerator
    {
        /// <summary>
        /// Correlativo global: toma el máximo valor numérico existente entre
        /// los códigos dados y devuelve el siguiente, con cero a la
        /// izquierda hasta completar <paramref name="length"/>.
        /// </summary>
        public static async Task<string> NextAsync(
            IQueryable<string> existingCodes,
            int length,
            string entityLabel,
            CancellationToken cancellationToken)
        {
            var codes = await existingCodes.ToListAsync(cancellationToken);

            var max = codes
                .Select(c => int.TryParse(c, out var n) ? n : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n!.Value)
                .DefaultIfEmpty(0)
                .Max();

            var next = max + 1;
            var maxAllowed = (int)Math.Pow(10, length) - 1;

            if (next > maxAllowed)
            {
                throw new InvalidOperationException(
                    $"No hay códigos disponibles para {entityLabel}: se alcanzó el máximo de " +
                    $"{maxAllowed} registros ({length} dígitos).");
            }

            return next.ToString().PadLeft(length, '0');
        }

        /// <summary>
        /// Correlativo scoped a un prefijo (ej. SubFamilia dentro de una
        /// Familia): toma el máximo sufijo numérico entre los códigos que
        /// empiezan con <paramref name="prefix"/> y devuelve
        /// prefix + siguiente correlativo, con cero a la izquierda.
        /// </summary>
        public static async Task<string> NextChildAsync(
            IQueryable<string> existingCodes,
            string prefix,
            int suffixLength,
            string entityLabel,
            CancellationToken cancellationToken)
        {
            var suffixes = await existingCodes
                .Where(c => c.StartsWith(prefix))
                .Select(c => c.Substring(prefix.Length))
                .ToListAsync(cancellationToken);

            var max = suffixes
                .Select(s => int.TryParse(s, out var n) ? n : (int?)null)
                .Where(n => n.HasValue)
                .Select(n => n!.Value)
                .DefaultIfEmpty(0)
                .Max();

            var next = max + 1;
            var maxAllowed = (int)Math.Pow(10, suffixLength) - 1;

            if (next > maxAllowed)
            {
                throw new InvalidOperationException(
                    $"No hay códigos disponibles para {entityLabel} dentro de '{prefix}': se alcanzó " +
                    $"el máximo de {maxAllowed} registros ({suffixLength} dígitos).");
            }

            return prefix + next.ToString().PadLeft(suffixLength, '0');
        }
    }
}