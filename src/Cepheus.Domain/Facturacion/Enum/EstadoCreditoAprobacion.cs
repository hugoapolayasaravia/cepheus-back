namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Resultado de Créditos en la aprobación (Aprobaciones.Estado_cre). Se guarda con la letra legacy:
    /// A Aprobado | R Rechazado. Con R la cantidad aprobada es 0 (PB: "if is_estC = 'R' then id_cantidad = 0").
    /// </summary>
    public enum EstadoCreditoAprobacion
    {
        Aprobado = 1,
        Rechazado = 2,
    }
}
