namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Qué productos incluye la Hoja de Corte. Legacy: dbo.RHojaCorte.TipoPro_hco
    /// (char(1)):
    ///   'V' -> Vigueta
    ///   'B' -> Bovedilla
    ///   '2' -> ViguetaYBovedilla ("Vig&amp;Bov" en la pantalla)
    /// Se persiste como ordinal.
    /// </summary>
    public enum TipoProductoHojaCorte
    {
        Vigueta = 0,
        Bovedilla = 1,
        ViguetaYBovedilla = 2
    }
}
