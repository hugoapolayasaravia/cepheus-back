namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Tipo de atributo de la especificación técnica del concreto. Cada valor
    /// corresponde a una columna COD_* de dbo.MProductos (legacy) y a una
    /// propiedad de Producto:
    ///   Resistencia (COD_RESISTENCIA -> StrengthCode)
    ///   TipoCemento (COD_TIP_CEMENTO -> CementTypeCode)
    ///   TamanoPiedra (COD_TAM_PIEDRA -> StoneSizeCode)
    ///   Slump (COD_SLUMP -> SlumpCode)
    ///   RelacionAguaCemento (COD_AGUA_CEMENTO -> WaterCementRatioCode)
    ///   Edad (COD_EDAD -> AgeCode)
    ///   CondicionEspecial (COD_COND_ESPECIAL -> SpecialConditionCode)
    ///   ProporcionMezcla (COD_PROP_MEZCLA -> MixProportionCode)
    ///
    /// Los valores parten en 1 (no en 0) para que un valor sin asignar no se
    /// confunda con un tipo real.
    /// </summary>
    public enum TipoAtributoConcreto
    {
        Resistencia = 1,
        TipoCemento = 2,
        TamanoPiedra = 3,
        Slump = 4,
        RelacionAguaCemento = 5,
        Edad = 6,
        CondicionEspecial = 7,
        ProporcionMezcla = 8
    }
}
