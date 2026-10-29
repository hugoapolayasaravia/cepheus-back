using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Responsable de obra: personas de contacto asociadas a una obra de un
    /// cliente (nombre, teléfono y cargo). Catálogo del módulo de Facturación
    /// y Ventas.
    ///
    /// Legacy: dbo.MObras_Responsables (SQL Server). PK compuesta de 3
    /// columnas (Codigo_Cli, Codigo_Obr, Codigo_Res) y FK compuesta hacia
    /// dbo.MObras (Codigo_Cli, Codigo_Obr). Se mantiene la PK compuesta:
    /// Codigo_Res es un correlativo por obra, no un código único global
    /// (mismo criterio que Obra respecto de Cliente).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Cli  -> ClienteCode (char(5), FK compuesta -> Obra, parte de la PK)
    ///   Codigo_Obr  -> ObraCode (char(3), FK compuesta -> Obra, parte de la PK)
    ///   Codigo_Res  -> Code (int, correlativo por obra, parte de la PK)
    ///   Nombre_Res  -> Name (varchar(50), NOT NULL)
    ///   Telefono_Res -> Phone (varchar(50), NOT NULL)
    ///   Cargo_Res   -> Position (varchar(50), NOT NULL)
    ///
    /// IsActive no existe en el legacy; no se agrega porque el responsable no
    /// tiene ciclo de vida propio en el legacy (se edita o se elimina la fila).
    ///
    /// Relación con Obra.ResponsibleName/ResponsiblePhone: la obra conserva
    /// su responsable principal en columnas propias (legacy MObras); esta
    /// tabla permite registrar contactos adicionales por obra. No se
    /// unifican porque el legacy las mantiene separadas.
    /// </summary>
    public class ResponsableObra : IAuditableEntity
    {
        public string ClienteCode { get; set; } = default!;
        public string ObraCode { get; set; } = default!;
        public Maestros.Obra Obra { get; set; } = default!;

        /// <summary>Correlativo por obra (Codigo_Res, int en el legacy).</summary>
        public int Code { get; set; }

        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Position { get; set; } = default!;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
