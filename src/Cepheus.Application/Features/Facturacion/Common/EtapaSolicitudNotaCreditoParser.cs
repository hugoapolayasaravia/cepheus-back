using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Common
{
    /// <summary>
    /// Convierte el texto recibido en la API (nombre del enum o su código numérico,
    /// ej. "Ventas" o "1") a EtapaSolicitudNotaCredito, rechazando valores fuera del enum.
    /// </summary>
    public static class EtapaSolicitudNotaCreditoParser
    {
        public static string ValidNames =>
            string.Join(", ", System.Enum.GetNames(typeof(EtapaSolicitudNotaCredito)));

        public static bool TryParse(string? value, out EtapaSolicitudNotaCredito etapa)
        {
            etapa = default;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return System.Enum.TryParse(value.Trim(), true, out etapa)
                && System.Enum.IsDefined(typeof(EtapaSolicitudNotaCredito), etapa);
        }

        public static EtapaSolicitudNotaCredito Parse(string value)
        {
            if (!TryParse(value, out var etapa))
            {
                throw new InvalidOperationException($"La etapa debe ser una de: {ValidNames}.");
            }

            return etapa;
        }
    }
}
