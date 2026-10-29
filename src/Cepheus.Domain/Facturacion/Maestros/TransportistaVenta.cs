using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Facturacion.Maestros
{
    /// <summary>
    /// Transportista (empresa o persona que presta el servicio de transporte de
    /// los despachos). Entidad maestra del módulo de Facturación y Ventas.
    ///
    /// NO confundir con Logistica.Maestros.Transportista: mismo concepto de nombre,
    /// pero tabla, datos y ciclo de vida distintos (schema "facturacion").
    ///
    /// Legacy: dbo.Ttransportistas (SQL Server, ~20 años de antigüedad).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_tra    -> Code (PK natural, char(4), correlativo. Se mantiene la
    ///                    longitud legacy para no romper referencias históricas)
    ///   nombre_tra    -> Name (era varchar(50) NULL; ahora obligatorio, 150)
    ///   direccion_tra -> Address (varchar(100) -> 200)
    ///   codigo_Ubi    -> UbigeoCode (FK -> Comunes.Ubigeo.Code, opcional)
    ///   tipo_doc      -> DocumentTypeCode (FK -> Comunes.TipoDocumento.Code, según la
    ///                    indicación del script legacy; era char(1) NULL, ahora
    ///                    obligatorio)
    ///   ruc_tra       -> DocumentNumber (era varchar(11) NULL; el nombre "ruc" engañaba
    ///                    porque tipo_doc admite otros documentos; ahora 20)
    ///   telefono_tra  -> Phone (varchar(20) -> 30)
    ///   email_tra     -> Email (varchar(20) -> 150: 20 caracteres no alcanzan para un
    ///                    correo real; los datos legacy pueden venir truncados)
    ///   cimtc_tra     -> MtcInternalCode (código interno del MTC; era NOT NULL, ahora
    ///                    opcional: los blancos del legacy se migran como NULL)
    ///
    /// Agregados frente al legacy: IsActive (la tabla no manejaba estado), auditoría
    /// (IAuditableEntity) y RowVersion.
    ///
    /// Regla nueva: UNIQUE(DocumentTypeCode, DocumentNumber), mismo criterio que
    /// Logistica.Transportista. El legacy solo tenía la PK, así que hay que validar
    /// duplicados antes de migrar (docs/migracion/Transportista_validaciones.sql).
    /// </summary>
    public class TransportistaVenta : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string DocumentTypeCode { get; set; } = default!;
        public TipoDocumento DocumentType { get; set; } = default!;

        public string DocumentNumber { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Address { get; set; }
        public string? UbigeoCode { get; set; }
        public Ubigeo? Ubigeo { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }

        public string? MtcInternalCode { get; set; }

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
