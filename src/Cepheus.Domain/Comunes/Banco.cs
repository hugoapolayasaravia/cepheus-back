using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes
{
    /// <summary>
    /// Banco (entidad financiera). Catálogo compartido entre módulos (cuentas
    /// bancarias de proveedores, clientes, etc.).
    ///
    /// No existía en ningún script legacy provisto — se crea nuevo, a pedido,
    /// para dar referencial integridad a MProveedorCuentas.BancoCodigo (que en
    /// el legacy era varchar(10) libre, sin catálogo).
    ///
    /// A diferencia de Moneda/TipoDocumento/Ubigeo (que usan Id int + Code como
    /// índice único), este catálogo sigue el patrón técnico de los catálogos de
    /// Logística: PK natural string con correlativo autogenerado, sin Id
    /// surrogate — así se pidió explícitamente al crearlo.
    /// </summary>
    public class Banco : IAuditableEntity
    {
        /// <summary>
        /// Código del banco (PK natural, 3 caracteres, correlativo).
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
