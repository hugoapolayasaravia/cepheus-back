namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Etapa de la OT (PedidoVentaResumen.Etapa_Ped / PB "estado"). Se guarda con la letra legacy.
    ///   P Pendiente | A Anulada | C Aprobada por Créditos | G Genera programación.
    /// </summary>
    public enum EtapaOrdenTrabajoVenta
    {
        Pendiente = 1,
        Anulada = 2,
        AprobadaCredito = 3,
        GeneraProgramacion = 4,
    }
}
