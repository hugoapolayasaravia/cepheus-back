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
    }
}
