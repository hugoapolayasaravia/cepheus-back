using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Rrhh.Catalogos
{
    /// <summary>
    /// Modo de pago de la remuneración del trabajador (transferencia, efectivo, cheque, etc.).
    /// Catálogo del módulo RRHH.
    ///
    /// Fuente: script RRHH provisto (rrhh.modo_pago). Se crea como catálogo propio,
    /// distinto de Logistica.Catalogos.FormaPago (que representa condiciones de
    /// pago/crédito de compras, con otra semántica y otros campos).
    ///
    /// Mapeo columnas script -> propiedades profesionales:
    ///   codigo  -> Code (PK natural, autogenerado correlativamente por la
    ///              aplicación — mismo patrón que Comunes.Banco / Logística.Familia)
    ///   nombre  -> Name
    ///   activo  -> IsActive
    /// </summary>
    public class ModoPago : IAuditableEntity
    {
        /// <summary>
        /// Código del modo de pago (PK natural, hasta 20 caracteres, correlativo).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

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