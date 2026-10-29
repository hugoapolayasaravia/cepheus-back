namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Guía de venta que sustenta la solicitud (cantidad a devolver/acreditar por guía).
    ///
    /// Legacy: dbo.DCreVentasSus. PK legacy (Codigo_snc, Ano_snc, Mes_snc, Numero_gve).
    ///   Numero_gve   -> GuideNumber (guía de venta; el modelo de Guías de Venta aún no
    ///                   está migrado, queda como número simple)
    ///   Cantidad_snc -> Quantity
    /// </summary>
    public class SolicitudNotaCreditoGuia
    {
        public string Code { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public SolicitudNotaCredito Solicitud { get; set; } = default!;

        public string GuideNumber { get; set; } = default!;
        public decimal Quantity { get; set; }
    }
}
