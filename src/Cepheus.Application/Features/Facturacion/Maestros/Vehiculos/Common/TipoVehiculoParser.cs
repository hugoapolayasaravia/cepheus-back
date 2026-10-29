using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common
{
    /// <summary>
    /// Convierte el tipo de vehículo recibido como texto (API) al enum TipoVehiculo.
    /// Acepta solo los NOMBRES del enum (sin distinguir mayúsculas); no acepta números,
    /// que System.Enum.TryParse dejaría pasar aunque no estén definidos.
    /// </summary>
    public static class TipoVehiculoParser
    {
        public static readonly string[] ValidNames = System.Enum.GetNames<TipoVehiculo>();

        public static bool TryParse(string? value, out TipoVehiculo result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var name = ValidNames.FirstOrDefault(n =>
                string.Equals(n, value.Trim(), StringComparison.OrdinalIgnoreCase));

            if (name is null)
            {
                return false;
            }

            result = System.Enum.Parse<TipoVehiculo>(name);
            return true;
        }

        /// <summary>
        /// Versión para Get/Update/Toggle: un tipo inválido significa que el vehículo
        /// no existe, por eso lanza KeyNotFoundException (404).
        /// </summary>
        public static TipoVehiculo ParseOrNotFound(string? value)
        {
            if (TryParse(value, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException(
                $"Tipo de vehículo '{value}' no válido. Valores permitidos: {string.Join(", ", ValidNames)}.");
        }
    }
}
