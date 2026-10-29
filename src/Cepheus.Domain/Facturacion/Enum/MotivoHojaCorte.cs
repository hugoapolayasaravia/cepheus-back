namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Motivo de una Hoja de Corte de tipo Reemplazo (05). Legacy:
    /// dbo.RHojaCorte.Motivo_hco (char(1)):
    ///   'C' -> Caducidad
    ///   'M' -> ModificacionMedidas  (Mod. Medidas)
    ///   'Q' -> ModificacionCantidades (Mod. Cantidades)
    /// Se persiste como ordinal.
    /// </summary>
    public enum MotivoHojaCorte
    {
        Caducidad = 0,
        ModificacionMedidas = 1,
        ModificacionCantidades = 2
    }
}
