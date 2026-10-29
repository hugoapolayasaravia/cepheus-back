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
        TotalVales = 12
    }
}
