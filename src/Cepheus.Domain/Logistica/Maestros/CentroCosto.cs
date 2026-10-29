using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Centro de costo, asociado a una planta. Entidad maestra del módulo de
    /// Logística.
    ///
    /// Legacy: dbo.TMCentroCosto (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Cos       -> Code (PK natural, char(3), correlativo)
    ///   Descripcion_Cos  -> Name
    ///   Codigo_Pla       -> PlantaCode (FK -> Comunes.Planta.Code; mismo
    ///                       criterio confirmado en ArticuloProveedor/
    ///                       ArticuloStock: Codigo_Pla = Planta acá, char2)
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos/maestros del sistema.
    /// </summary>
    public class CentroCosto : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

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
