namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Códigos legacy del catálogo TipoHojaCorte que gobiernan reglas del
    /// documento (w_ges_reg_hoja_corte). Se centralizan acá para que ningún
    /// handler repita literales como "03" o "05".
    /// </summary>
    public static class TipoHojaCorteCodes
    {
        public const string Nuevo = "01";
        public const string Adicional = "03";
        public const string Reposicion = "04";
        public const string Reemplazo = "05";

        /// <summary>
        /// Adicional (03) y Reemplazo (05) se generan a partir de otra Hoja de Corte
        /// y exigen indicar la HC padre (PB: "Seleccione Hoja de Corte padre...").
        /// </summary>
        public static bool RequiresParent(string code)
            => code == Adicional || code == Reemplazo;

        /// <summary>Reemplazo (05) exige además el motivo (PB: "Seleccione Motivo...").</summary>
        public static bool RequiresMotivo(string code)
            => code == Reemplazo;
    }
}
