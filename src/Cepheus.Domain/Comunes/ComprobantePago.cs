// Cepheus.Domain/Comunes/ComprobantePago.cs  (reemplaza el archivo completo)
using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes
{
    /// <summary>
    /// Comprobante de pago (Factura, Boleta, Recibo por Honorarios, Nota de
    /// Crédito, etc.). Catálogo maestro compartido entre módulos (Ventas,
    /// Compras, Facturación).
    ///
    /// Legacy: tabla "comprobante_pago" (MySQL) + dbo.ttipdoc (SQL Server,
    /// solo los flags tributarios — ver nota abajo).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo               -> Code
    ///   codigo_sunat         -> SunatCode        (catálogo 01 SUNAT: 01=Factura, 03=Boleta, etc.)
    ///   nombre               -> Name
    ///   abreviatura          -> ShortName
    ///   descripcion          -> Description
    ///   requiere_ruc         -> RequiresRuc
    ///   requiere_direccion   -> RequiresAddress
    ///   estado               -> IsActive
    ///
    /// Cambio de diseño (ver Logística > Transacciones > OrdenCompra): los
    /// flags tributarios de dbo.ttipdoc, que originalmente se habían puesto
    /// en Comunes.TipoDocumento, se trasladan aquí — TipoDocumento es
    /// documento de identidad (RUC/DNI/CE), no comprobante de pago, y esta
    /// es la entidad correcta para esa lógica:
    ///   igv_tdo        -> AffectsIgv          (¿afecta IGV?)
    ///   nograbable_tdo -> IsNonTaxable        (¿es no gravable?)
    ///   renta_tdo      -> AffectsIncomeTax    (¿sujeto a retención de renta?)
    ///   fonavi_tdo     -> AffectsFonavi       (derogado en Perú, se mantiene por continuidad histórica)
    ///   servicio_tdo   -> IsService
    ///   igvext_tdo     -> AffectsForeignIgv
    ///   En_ocompra     -> AvailableForPurchaseOrder (¿seleccionable en una Orden de Compra?)
    ///
    /// AffectsIncomeTax/AffectsFonavi alimentan el cálculo de
    /// OrdenCompra.RentaCompra/FonaviCompra (ver OrdenCompraTotalsCalculator).
    /// </summary>
    public class ComprobantePago : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string SunatCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ShortName { get; set; } = default!;
        public string? Description { get; set; }

        public bool RequiresRuc { get; set; }
        public bool RequiresAddress { get; set; }

        public bool AffectsIgv { get; set; }
        public bool IsNonTaxable { get; set; }
        public bool AffectsIncomeTax { get; set; }
        public bool AffectsFonavi { get; set; }
        public bool IsService { get; set; }
        public bool AffectsForeignIgv { get; set; }
        public bool AvailableForPurchaseOrder { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}