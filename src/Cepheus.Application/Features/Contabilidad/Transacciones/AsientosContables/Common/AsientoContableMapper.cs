using Cepheus.Domain.Contabilidad.Transacciones;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common
{
    public static class AsientoContableMapper
    {
        /// <summary>Convierte soles a dólares con el tipo de cambio del asiento (0 si no hay tipo de cambio).</summary>
        public static decimal ToDol(decimal sol, decimal tipoCambio)
            => tipoCambio > 0 ? Math.Round(sol / tipoCambio, 2, MidpointRounding.AwayFromZero) : 0m;

        public static AsientoContableResponse Map(AsientoContable a)
        {
            var detalles = a.Detalles
                .OrderBy(d => d.ItemNumber)
                .Select(d => new AsientoContableDetalleResponse
                {
                    ItemNumber = d.ItemNumber,
                    PlantaCode = d.PlantaCode,
                    EsDestino = d.EsDestino,
                    Descripcion = d.Descripcion,
                    CuentaContableCode = d.CuentaContableCode,
                    AnexoTipoCode = d.AnexoTipoCode,
                    AnexoCode = d.AnexoCode,
                    NumeroDocumento = d.NumeroDocumento,
                    CargoSol = d.CargoSol,
                    AbonoSol = d.AbonoSol,
                    CargoDol = ToDol(d.CargoSol, a.TipoCambio),
                    AbonoDol = ToDol(d.AbonoSol, a.TipoCambio),
                    TipoContabilidad = d.TipoContabilidad,
                    PlantaEjecutoraCode = d.PlantaEjecutoraCode,
                    ComprobantePagoCode = d.ComprobantePagoCode
                }).ToList();

            return new AsientoContableResponse
            {
                Codigo = a.Codigo,
                Anio = a.Anio,
                Mes = a.Mes,
                TipoAsientoCode = a.TipoAsientoCode,
                TipoAsientoName = a.TipoAsiento?.Name,
                Valuation = a.TipoAsiento?.Valuation,
                Numero = a.Numero,
                Descripcion = a.Descripcion,
                FechaProceso = a.FechaProceso,
                TipoCambio = a.TipoCambio,
                TotalCargosSol = a.TotalCargosSol,
                TotalAbonosSol = a.TotalAbonosSol,
                TotalCargosDol = detalles.Sum(x => x.CargoDol),
                TotalAbonosDol = detalles.Sum(x => x.AbonoDol),
                Estado = a.Estado,
                Origen = a.Origen,
                Detalles = detalles,
                CreatedAt = a.CreatedAt,
                CreatedBy = a.CreatedBy,
                UpdatedAt = a.UpdatedAt,
                RowVersion = a.RowVersion
            };
        }

        public static AsientoContableListItemResponse MapListItem(AsientoContable a) => new()
        {
            Codigo = a.Codigo,
            Anio = a.Anio,
            Mes = a.Mes,
            TipoAsientoCode = a.TipoAsientoCode,
            Numero = a.Numero,
            Descripcion = a.Descripcion,
            FechaProceso = a.FechaProceso,
            TotalCargosSol = a.TotalCargosSol,
            TotalAbonosSol = a.TotalAbonosSol,
            Estado = a.Estado,
            Origen = a.Origen,
            CreatedBy = a.CreatedBy
        };
    }
}
