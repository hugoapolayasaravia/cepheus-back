namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common
{
    /// <summary>Datos que el motor de plantillas necesita para armar el asiento, ya expresados en soles.</summary>
    public sealed class AsientoFuente
    {
        public string PlantaCode { get; init; } = default!;
        public string Glosa { get; init; } = default!;
        public string? NumeroDocumento { get; init; }
        public string? ComprobantePagoCode { get; init; }
        public bool EsNotaCredito { get; init; }

        /// <summary>Moneda (ISO 4217) del documento. Solo provisión de ventas: filtra las líneas de la plantilla por moneda.</summary>
        public string? MonedaCode { get; init; }
        /// <summary>Cliente del documento de venta (solo provisión de ventas).</summary>
        public string? ClienteCode { get; init; }

        public string? ProveedorCode { get; init; }
        public string? ProveedorNombre { get; init; }
        /// <summary>Cuenta por pagar del proveedor según la moneda del documento (solo provisión).</summary>
        public string? CuentaPorPagarCode { get; init; }

        public decimal Total { get; init; }
        public decimal Igv { get; init; }
        public decimal IgvExterior { get; init; }
        public decimal Renta { get; init; }
        public decimal Fonavi { get; init; }
        public decimal NoGravable { get; init; }
        public decimal Servicio { get; init; }
        public decimal MontoNotaIngreso { get; init; }
        public decimal NetoNotaIngreso { get; init; }

        /// <summary>Cuenta de caja o banco del pago (solo cancelación de ventas).</summary>
        public string? CuentaPagoCode { get; init; }
        /// <summary>Total recibido en soles (solo cancelación de ventas).</summary>
        public decimal TotalPago { get; init; }
        /// <summary>Importe cobrado de documentos en cobranza dudosa, en soles (solo cancelación de ventas).</summary>
        public decimal CobranzaDudosa { get; init; }
        /// <summary>Diferencia de cambio a favor, en soles (solo cancelación de ventas).</summary>
        public decimal DiferenciaGanancia { get; init; }
        /// <summary>Diferencia de cambio en contra, en soles (solo cancelación de ventas).</summary>
        public decimal DiferenciaPerdida { get; init; }

        public List<AsientoFuenteLinea> Lineas { get; init; } = new();
    }

    public sealed class AsientoFuenteLinea
    {
        public string PlantaCode { get; init; } = default!;
        public string Detalle { get; init; } = default!;
        public string CuentaCode { get; init; } = default!;
        public string? AnexoTipoCode { get; init; }
        public string? AnexoCode { get; init; }
        public decimal Parcial { get; init; }

        /// <summary>Número del documento de la línea; si se informa reemplaza al de la fuente (cancelación: documento cancelado).</summary>
        public string? NumeroDocumento { get; init; }

        /// <summary>La línea toma la naturaleza contraria a la de la plantilla (cancelación: nota de crédito).</summary>
        public bool InvierteNaturaleza { get; init; }
    }
}
