using Cepheus.Domain.Comun;
using Cepheus.Domain.Contabilidad.Catalogos;

namespace Cepheus.Domain.Contabilidad.Maestros
{
    /// <summary>
    /// Anexo contable. Maestro del módulo de Contabilidad, clasificado por TipoAnexo.
    ///
    /// Legacy: dbo.MAnexos (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tip       -> TipoAnexoCode (parte de la PK; FK -> TipoAnexo.Code, char(3))
    ///   Codigo_ane       -> Code (parte de la PK, char(5); correlativo por tipo de anexo)
    ///   Descripcion_ane  -> Name (varchar(150), NULL permitido tal cual el legacy)
    ///   Enlace1_cta      -> LinkAccount1Code (char(8), NOT NULL; SIN relación, tal cual
    ///                       el legacy)
    ///   Enlace2_cta      -> LinkAccount2Code (char(8), NOT NULL; SIN relación)
    ///   Estado           -> IsActive (el legacy usaba TEstados con solo '05' Activo y
    ///                       '06' Inactivo; dos valores = bool, mismo criterio que
    ///                       Vendedor y Cobrador)
    ///   flag             -> Flag (char(1), NULL; "mantener" en el legacy, se conserva)
    /// </summary>
    public class Anexo : IAuditableEntity
    {
        public string TipoAnexoCode { get; set; } = default!;
        public TipoAnexo TipoAnexo { get; set; } = default!;

        /// <summary>Código del anexo (5 caracteres, correlativo dentro de cada tipo).</summary>
        public string Code { get; set; } = default!;

        public string? Name { get; set; }

        public string LinkAccount1Code { get; set; } = string.Empty;
        public string LinkAccount2Code { get; set; } = string.Empty;

        public string? Flag { get; set; }

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
