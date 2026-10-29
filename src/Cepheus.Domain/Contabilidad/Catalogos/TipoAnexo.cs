using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Contabilidad.Catalogos
{
    /// <summary>
    /// Tipo de anexo contable. Catálogo del módulo de Contabilidad: clasifica los
    /// anexos (MAnexos) y se asigna a cada cuenta contable (MCuentas.Codigo_tip).
    ///
    /// Legacy: dbo.Ttipane (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tip      -> Code (PK natural, char(3), correlativo automático)
    ///   Descripcion_tip -> Name (nvarchar(50) -> 100)
    ///   Estado_tip      -> IsActive (char(1): no puede referenciar TEstados, cuyos
    ///                      códigos son de 2 caracteres; es un indicador activo/inactivo)
    ///   tipo_tip        -> Flag (char(1), opcional; el legacy lo marcaba "mantener" y
    ///                      no documenta sus valores: se conserva tal cual hasta
    ///                      confirmar su significado, luego pasaría a enum)
    /// </summary>
    public class TipoAnexo : IAuditableEntity
    {
        /// <summary>Código del tipo de anexo (PK natural, 3 caracteres, correlativo).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        /// <summary>Flag del tipo de anexo (legacy tipo_tip; significado por confirmar).</summary>
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
