using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Motivo de rebote de un cheque de cliente (catálogo).
    ///
    /// Legacy: dbo.TMotCheReb.
    ///   Codigo_mch      -> Code (PK natural, char(2), correlativo)
    ///   Descripcion_mch -> Name (varchar(50))
    /// </summary>
    public class MotivoChequeRebote : IAuditableEntity
    {
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
