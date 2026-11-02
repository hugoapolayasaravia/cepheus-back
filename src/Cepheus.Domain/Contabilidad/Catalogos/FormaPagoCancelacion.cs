using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Contabilidad.Catalogos
{
    /// <summary>
    /// Forma de pago de la cancelación a proveedores (cómo se paga el cheque/operación).
    ///
    /// Legacy: dbo.COM_FOR_PAG_CANCELACION (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   COD_FOR_PAG_CANCELACION -> Code (PK natural, char(1); se digita: 1, 2, 3)
    ///   DES_FOR_PAG_CANCELACION -> Name (varchar(20) -> 50)
    ///   codigo_est              -> IsActive (TEstados solo se usaba como Activo/Inactivo)
    ///
    /// Regla del proceso (se aplica en la cancelación, no en esta tabla): al elegir el banco,
    /// si el proveedor tiene una cuenta en ese banco se asigna la forma "3" y se exige la cuenta
    /// del proveedor; si no la tiene, se asigna la forma "2".
    /// </summary>
    public class FormaPagoCancelacion : IAuditableEntity
    {
        /// <summary>Código de la forma de pago (PK natural, 1 carácter).</summary>
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
