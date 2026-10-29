using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Dirección de un proveedor (tabla de detalle, N direcciones por proveedor).
    ///
    /// Legacy: dbo.MProveedorDirecciones (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   IdDireccion    -> Id (PK real, IDENTITY — no es un código de negocio,
    ///                     a diferencia de los catálogos)
    ///   Codigo_prv     -> ProveedorCode (FK -> Proveedor.Code)
    ///   TipoDireccion  -> AddressType (enum)
    ///   Direccion      -> Address
    ///   CodigoUbigeo   -> UbigeoCode (FK -> Comunes.Ubigeo.Code, nullable)
    ///   Referencia     -> Reference
    ///   EsPrincipal    -> IsPrimary (solo una dirección principal por
    ///                     proveedor; exclusividad manejada en el handler)
    ///   Estado         -> reemplazado por IsActive estándar
    /// </summary>
    public class ProveedorDireccion : IAuditableEntity
    {
        public int Id { get; set; }

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public AddressType AddressType { get; set; }

        public string Address { get; set; } = default!;

        public string? UbigeoCode { get; set; }
        public Ubigeo? Ubigeo { get; set; }

        public string? Reference { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
