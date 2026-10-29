using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Catalogos;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Condición comercial de compra con un proveedor (tabla de detalle, N
    /// condiciones por proveedor — ej. una por moneda o por línea de compra).
    ///
    /// Legacy: dbo.MProveedorCondiciones (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   IdCondicion      -> Id (PK real, IDENTITY)
    ///   Codigo_prv       -> ProveedorCode (FK -> Proveedor.Code)
    ///   CodigoFormaPago  -> FormaPagoCode (FK -> Logistica.Catalogos.FormaPago.Code,
    ///                        catálogo ya existente — sin ambigüedad)
    ///   PlazoPago        -> PaymentTermDays (plazo específico de ESTA condición;
    ///                        puede diferir del Days por defecto de la FormaPago)
    ///   Moneda           -> MonedaCode (FK -> Comunes.Moneda.Code)
    ///   LimiteCredito    -> CreditLimit (nullable)
    ///   Descuento        -> DiscountPercentage (nullable)
    ///   EsPrincipal      -> IsPrimary (una sola condición principal por
    ///                        proveedor; exclusividad manejada en el handler.
    ///                        Nota: el legacy la define en DEFAULT 1, a
    ///                        diferencia de Direcciones/Contactos/Cuentas que
    ///                        default en 0 — se respeta esa diferencia)
    ///   Estado           -> reemplazado por IsActive estándar
    /// </summary>
    public class ProveedorCondicion : IAuditableEntity
    {
        public int Id { get; set; }

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string FormaPagoCode { get; set; } = default!;
        public FormaPago FormaPago { get; set; } = default!;

        public int PaymentTermDays { get; set; }

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public decimal? CreditLimit { get; set; }
        public decimal? DiscountPercentage { get; set; }

        public bool IsPrimary { get; set; } = true;

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
