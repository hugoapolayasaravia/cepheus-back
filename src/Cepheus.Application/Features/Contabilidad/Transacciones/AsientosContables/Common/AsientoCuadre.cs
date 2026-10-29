using Cepheus.Domain.Contabilidad.Transacciones;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common
{
    /// <summary>
    /// Totaliza el asiento y exige que cuadre (Debe = Haber). El legacy solo sumaba los totales sin validar.
    /// </summary>
    public static class AsientoCuadre
    {
        public static void Totalize(AsientoContable asiento)
        {
            asiento.TotalCargosSol = Math.Round(asiento.Detalles.Sum(d => d.CargoSol), 2, MidpointRounding.AwayFromZero);
            asiento.TotalAbonosSol = Math.Round(asiento.Detalles.Sum(d => d.AbonoSol), 2, MidpointRounding.AwayFromZero);
        }

        public static void EnsureBalanced(AsientoContable asiento)
        {
            Totalize(asiento);

            if (asiento.TotalCargosSol != asiento.TotalAbonosSol)
            {
                throw new InvalidOperationException(
                    $"El asiento no cuadra: Debe {asiento.TotalCargosSol:N2} y Haber {asiento.TotalAbonosSol:N2} " +
                    $"(diferencia {asiento.TotalCargosSol - asiento.TotalAbonosSol:N2}).");
            }
        }
    }
}
