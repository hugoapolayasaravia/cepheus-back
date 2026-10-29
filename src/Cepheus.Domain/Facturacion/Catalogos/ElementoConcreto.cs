using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Elemento de concreto (catálogo simple). Usado en el módulo de Facturación.
    ///
    /// Legacy: dbo.TElemento (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Ele  -> Code (PK natural, char(2))
    ///   Nombre_Ele  -> Name (varchar(50))
    ///   Codigo_Est  -> IsActive (char(2), default '05'; reemplazado por bandera booleana)
    ///   Flag_Saldo  -> UsesBalance (char(1), default 'S'; 'S' = true)
    ///
    /// El significado exacto de Flag_Saldo en el legacy no está documentado; se conserva
    /// como UsesBalance (default true).
    /// </summary>
    public class ElementoConcreto : IAuditableEntity
    {
        /// <summary>
        /// Código del elemento de concreto (PK natural, 2 caracteres).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        /// <summary>
        /// Legacy Flag_Saldo ('S'/'N'). Default true.
        /// </summary>
        public bool UsesBalance { get; set; } = true;

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
