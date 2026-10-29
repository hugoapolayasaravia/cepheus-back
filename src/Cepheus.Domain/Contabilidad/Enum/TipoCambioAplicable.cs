namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Tipo de cambio aplicable a un tipo de asiento. Corresponde a las columnas de
    /// Comunes.TipoCambio (BuyRate / SellRate).
    ///
    /// Legacy: dbo.TTipAsiento.TipodeCambio_tas (char(1)):
    ///   'C' -> Compra
    ///   'V' -> Venta
    ///
    /// Catálogo cerrado de 2 valores -> enum, mismo criterio que TurnoHojaCorte. Se
    /// persiste como int (ordinal); el mapeo letra -> valor se resuelve en el ETL.
    /// Los valores parten en 1 para que un valor sin asignar (0) nunca se confunda
    /// con un tipo real.
    /// </summary>
    public enum TipoCambioAplicable
    {
        Compra = 1,
        Venta = 2
    }
}
