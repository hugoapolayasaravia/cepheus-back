using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Catalogos;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Motivo de Solicitud de Nota de Crédito de Ventas (catálogo). Clasifica el
    /// motivo por el cual se solicita una nota de crédito y lo relaciona con el
    /// motivo de devolución de Comunes (efecto contable/stock).
    ///
    /// Legacy: dbo.TMotivo_snc (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_mnc   -> Code (PK natural, char(2), correlativo)
    ///   Nombre_msc   -> Name (varchar(80))
    ///   Tipo_msc     -> ParentCode (auto-FK -> Codigo_mnc: motivo "padre"/grupo
    ///                   al que pertenece este motivo; opcional)
    ///   IND_ESTADO   -> IsActive
    ///   FEC_INICIO   -> StartDate (inicio de vigencia, opcional)
    ///   FEC_FIN      -> EndDate (fin de vigencia, opcional)
    ///   codigo_mot   -> MotivoDevolucionCode (FK -> Comunes.MotivoDevolucion,
    ///                   que reemplaza a dbo.Tmotivos; opcional)
    ///   afecta_mnc   -> AffectsStock (char(1) S/N -> bool; SUPUESTO: mismo
    ///                   significado que MotivoDevolucion.AffectsStock)
    /// </summary>
    public class MotivoSolicitudNotaCredito : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;

        public string? ParentCode { get; set; }
        public MotivoSolicitudNotaCredito? Parent { get; set; }

        public string? MotivoDevolucionCode { get; set; }
        public MotivoDevolucionArticulo? MotivoDevolucion { get; set; }

        public bool AffectsStock { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

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
