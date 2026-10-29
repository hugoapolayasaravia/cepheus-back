using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes
{
    /// <summary>
    /// Ubigeo: código de ubicación geográfica (Departamento/Provincia/Distrito)
    /// según el catálogo del INEI/RENIEC usado en Perú. Catálogo maestro nacional
    /// compartido entre módulos (Comunes.Planta, direcciones de clientes/proveedores,
    /// comprobantes electrónicos SUNAT, etc.).
    ///
    /// Legacy: dbo.TUbigeo (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_ubi        -> Code               (6 dígitos: 2 depto + 2 prov + 2 distrito)
    ///   departamento_ubi  -> Department
    ///   provincia_ubi     -> Province
    ///   distrito_ubi      -> District
    ///   direccion         -> FullAddress (NO persistido; era una columna computada
    ///                          en SQL Server que concatenaba depto+prov+distrito.
    ///                          Se recalcula en memoria vía propiedad de solo lectura
    ///                          en vez de columna calculada en base de datos, para no
    ///                          atar la lógica de formato al motor de BD.)
    ///
    /// IsActive no existe en la tabla legacy (TUbigeo no maneja soft-status); se
    /// agrega por consistencia con el resto de catálogos del sistema (mismo caso
    /// que TipoDocumento).
    ///
    /// Nota operativa: a diferencia de los catálogos anteriores, Ubigeo es una
    /// tabla nacional extensa (~1,800+ registros en Perú) que normalmente se carga
    /// una sola vez vía importación/seed (INEI/RENIEC), no registro manual uno a
    /// uno desde la UI. Igual expongo Create/Update por consistencia de patrón y
    /// para correcciones puntuales, pero el flujo principal de carga masiva debería
    /// resolverse con un importador (CSV/seed), no con estos endpoints.
    /// </summary>
    public class Ubigeo : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string Department { get; set; } = default!;
        public string Province { get; set; } = default!;
        public string District { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;

        /// <summary>
        /// Equivalente a la columna computada "direccion" del legacy. Solo lectura,
        /// no mapeada a base de datos (ver comentario de clase).
        /// </summary>
        public string FullAddress => $"{Department} {Province} {District}".Trim();
    }
}