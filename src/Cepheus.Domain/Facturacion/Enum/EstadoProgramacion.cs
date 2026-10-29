namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de una programación de Concreto u Otros (Concreto.codigo_est / Otros). El valor numérico es el código legacy:
    /// 01 Pendiente | 04 Anulada | 11 Cerrada.
    /// </summary>
    public enum EstadoProgramacion
    {
        Pendiente = 1,
        Anulada = 4,
        Cerrada = 11,
    }
}
