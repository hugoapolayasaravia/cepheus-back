using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Comunes
{
    /// <summary>
    /// Planta / sede de trabajo (ej. Planta Villa, Planta Santa Anita).
    /// Entidad maestra compartida entre módulos (Logística, Ventas, Compras, etc.).
    ///
    /// Legacy: dbo.MPlantas (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_pla        -> Code
    ///   Descripcion_pla   -> Name
    ///   Razon_Pla         -> LegalName
    ///   direccion_pla    -> Address
    ///   direccion_pla2   -> AddressComplement
    ///   direccion_ubi    -> UbigeoCode
    ///   responsable_pla   -> ManagerName
    ///   almacen           -> HasWarehouse      ('S'/'N' -> bool)
    ///   Produccion        -> IsProductionPlant ('S'/'N' -> bool)
    ///   proyecto_sn       -> IsProject         ('S'/'N' -> bool)
    ///   flg_aprobaciones  -> RequiresApprovals
    ///   detraccion_pla    -> AppliesDetraction
    ///   codigo_est        -> StatusCode
    ///   Estado_pla        -> IsActive           ('A' -> true)
    ///
    /// Numeración de documentos emitidos por cada planta:
    ///   numero_gve        -> NumeroGve
    ///   numero_fve        -> NumeroFve
    ///   numero_dve        -> NumeroDve
    ///   numero_bve        -> NumeroBve
    ///   numero_cve        -> NumeroCve
    ///   numero_let        -> NumeroLet
    ///   numero_ret        -> NumeroRet
    ///   guia_num          -> GuiaNum
    ///
    /// La numeración pertenece a la planta debido a que cada planta
    /// mantiene sus propios correlativos para los documentos que emite.
    /// Los nuevos registros utilizan "000-00000" como numeración inicial.
    /// </summary>
    public class Planta : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? LegalName { get; set; }

        public string Address { get; set; } = default!;
        public string? AddressComplement { get; set; }
        public string? UbigeoCode { get; set; }

        public string? ManagerName { get; set; }

        public bool HasWarehouse { get; set; }
        public bool IsProductionPlant { get; set; }
        public bool IsProject { get; set; }
        public bool RequiresApprovals { get; set; }
        public bool AppliesDetraction { get; set; }

        public string? StatusCode { get; set; }

        // Numeración de documentos emitidos por la planta
        public string NumeroGve { get; set; } = "000-00000";
        public string NumeroFve { get; set; } = "000-00000";
        public string NumeroDve { get; set; } = "000-00000";
        public string NumeroBve { get; set; } = "000-00000";
        public string NumeroCve { get; set; } = "000-00000";
        public string NumeroLet { get; set; } = "000-00000";
        public string NumeroRet { get; set; } = "000-00000";
        public string GuiaNum { get; set; } = "000-00000";

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