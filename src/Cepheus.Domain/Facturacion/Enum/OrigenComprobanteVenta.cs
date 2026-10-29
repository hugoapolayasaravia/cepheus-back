namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Cómo nace el comprobante: Guias (tipo_fve N/P/G/C: detalle tomado de guías de remisión) o Libre (tipo 'O':
    /// consumos varios sin guía ni aprobación de Créditos).
    /// </summary>
    public enum OrigenComprobanteVenta
    {
        Guias = 1,
        Libre = 2,
    }
}
