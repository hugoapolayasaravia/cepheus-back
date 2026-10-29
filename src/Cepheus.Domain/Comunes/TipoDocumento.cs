// Cepheus.Domain/Comunes/TipoDocumento.cs  (reemplaza el archivo completo)
using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes
{
    /// <summary>
    /// Tipo de documento de identidad (RUC/DNI/CE/etc.) — catálogo compartido
    /// usado por Proveedor, Trabajador y otras entidades que necesitan
    /// identificar personas o empresas.
    ///
    /// Legacy: dbo.Ttipdoc (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tdo        -> Code
    ///   Descripcion_tdo   -> Name
    ///   DesAbreviada_tdo  -> ShortName
    ///   sunat_tdo         -> SunatCode
    ///
    /// Nota (cambio de diseño): los flags tributarios (igv_tdo,
    /// nograbable_tdo, renta_tdo, fonavi_tdo, servicio_tdo, igvext_tdo,
    /// En_ocompra) que originalmente se pensaron para esta entidad se
    /// trasladaron a Comunes.ComprobantePago — TipoDocumento ya se usa en
    /// varias entidades como documento de identidad (Proveedor, RRHH
    /// TipoDocumentoIdentidad, etc.) y mezclar ambos conceptos generaba
    /// confusión. Ver ComprobantePago para esa lógica.
    /// </summary>
    public class TipoDocumento : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ShortName { get; set; }
        public string? SunatCode { get; set; }

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