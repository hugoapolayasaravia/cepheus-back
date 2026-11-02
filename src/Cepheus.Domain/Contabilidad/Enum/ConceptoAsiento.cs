namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Concepto (origen del importe) de una línea de plantilla. El motor de generación resuelve el importe
    /// de cada concepto a partir del documento de compra o de la nota de ingreso.
    ///
    ///   TotalDocumento    : total del documento de compra (DocumentoCompra.TotalSol)
    ///   DetalleDocumento  : una línea del asiento por cada línea del documento (ParcialSol; el servicio se suma
    ///                       a la primera, como en el legacy)
    ///   Igv               : IGV del documento (IgvSol)
    ///   IgvExterior       : IGV exterior (IgvExteriorSol)
    ///   Renta             : retención de renta (RentaSol)
    ///   Fonavi            : fonavi (FonaviSol)
    ///   NoGravable        : no gravable (NoGravableSol). El legacy NO lo contabiliza (línea comentada)
    ///   Servicio          : servicio (ServicioSol). El legacy NO lo contabiliza como línea propia
    ///   MontoNotaIngreso  : NotaIngreso.Monto (monto total de la nota de ingreso)  [solo transferencias]
    ///   NetoNotaIngreso   : NotaIngreso.Total (valor neto de la nota de ingreso)   [solo transferencias]
    /// </summary>
    public enum ConceptoAsiento
    {
        TotalDocumento = 1,
        DetalleDocumento = 2,
        Igv = 3,
        IgvExterior = 4,
        Renta = 5,
        Fonavi = 6,
        NoGravable = 7,
        Servicio = 8,
        MontoNotaIngreso = 9,
        NetoNotaIngreso = 10,

        /// <summary>Una línea del asiento por cada línea de vale de salida (importe = total de la línea). Solo ConsumoVales.</summary>
        DetalleVale = 11,

        /// <summary>Suma de los importes de las líneas de vale (se asienta una sola vez). Solo ConsumoVales.</summary>
        TotalVales = 12,
        /// <summary>Una línea del asiento por cada documento del pago (importe = pago del documento, en positivo). Solo CancelacionProveedor.</summary>
        PagoDocumento = 13,
        /// <summary>Retención del IGV del pago (solo si es mayor a 0). Solo CancelacionProveedor.</summary>
        PagoRetencion = 14,
        /// <summary>Total a pagar del pago (pago menos retención), en la cuenta del banco. Solo CancelacionProveedor.</summary>
        PagoTotal = 15,
        /// <summary>Total recibido en la cancelación, en soles (caja, banco o letras). Solo CancelacionVenta; el canje no lo genera.</summary>
        PagoRecibido = 16,
        /// <summary>Importe cobrado de documentos en cobranza dudosa. Solo CancelacionVenta; se genera solo si es mayor a 0.</summary>
        CobranzaDudosa = 17,
        /// <summary>Diferencia de cambio a favor (lo que falta al Haber para cuadrar). Solo CancelacionVenta; solo si es mayor a 0.</summary>
        DiferenciaCambioGanancia = 18,
        /// <summary>Diferencia de cambio en contra (lo que falta al Debe para cuadrar). Solo CancelacionVenta; solo si es mayor a 0.</summary>
        DiferenciaCambioPerdida = 19




    }
}
