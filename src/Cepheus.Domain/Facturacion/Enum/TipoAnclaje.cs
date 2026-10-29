namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Tipo de anclaje de la vigueta, determina cómo se calcula
    /// LongitudExt_Cot (OuterLength) a partir de LongitudInt_Cot (InnerLength)
    /// y Apoyo_Cot (Support).
    ///
    /// Legacy: dbo.CotizacionesMetradoDet.Anclaje_Cot (char(1)). CORRECCIÓN:
    /// en una versión anterior de este modelo se mapeó como bool
    /// (HasAnchorage), asumiendo un simple Sí/No. El código fuente de la
    /// ventana original (w_cotizaciones, choose case ls_anc) muestra que en
    /// realidad son 3 valores:
    ///   'S' -> Si:    OuterLength = InnerLength + Support
    ///   'N' -> No:    OuterLength = InnerLength (Support no se suma)
    ///   'L' -> Medio: OuterLength = InnerLength + Support/2
    ///
    /// PENDIENTE CONFIRMAR: el significado exacto de 'L' (se nombra "Medio"
    /// tentativamente, por ser la mitad del apoyo de 'S'; el código no trae
    /// un comentario que lo aclare literalmente).
    /// </summary>
    public enum TipoAnclaje
    {
        Si = 1,
        No = 2,
        Medio = 3
    }
}
