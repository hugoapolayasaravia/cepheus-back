namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common
{
    public static class AsientoContableRules
    {
        /// <summary>Tipo de asiento 04 = Apertura: no puede anularse (regla del legacy).</summary>
        public const string TipoAsientoApertura = "04";

        public const int CodigoLength = 13;
        public const int NumeroLength = 5;

        public static (string Anio, string Mes, string TipoAsientoCode, string Numero) ParseCodigo(string codigo)
        {
            var c = (codigo ?? string.Empty).Trim();
            if (c.Length != CodigoLength)
            {
                throw new ArgumentException(
                    "El código del asiento debe tener 13 caracteres (año 4 + mes 2 + tipo 2 + número 5).");
            }

            return (c.Substring(0, 4), c.Substring(4, 2), c.Substring(6, 2), c.Substring(8, 5));
        }
    }
}
