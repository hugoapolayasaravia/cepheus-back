using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Vehículo de un transportista. Entidad maestra del módulo de
    /// Logística.
    ///
    /// Legacy: dbo.MVehiculos (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   CodigoVehiculo             -> Code (PK natural, char(5), correlativo)
    ///   CodigoTransportista        -> TransportistaCode (FK -> Transportista.Code)
    ///   Placa                      -> LicensePlate (único)
    ///   CategoriaVehiculo          -> VehicleCategory (texto libre — sin
    ///                                 catálogo, dominio abierto)
    ///   TipoVehiculo               -> VehicleType (texto libre, mismo criterio)
    ///   Marca                      -> Brand
    ///   Modelo                     -> Model
    ///   AnioFabricacion            -> ManufactureYear
    ///   NumeroMotor                -> EngineNumber
    ///   NumeroChasis               -> ChassisNumber
    ///   Color                      -> Color
    ///   CapacidadCargaKg           -> CargoCapacityKg
    ///   LargoM, AnchoM, AltoM      -> LengthM, WidthM, HeightM
    ///   NumeroCertificadoVehicular -> VehicularCertificateNumber
    ///   NumeroTarjetaCirculacion   -> CirculationCardNumber
    ///   ConfiguracionVehicular     -> VehicularConfiguration
    ///   Estado                     -> reemplazado por IsActive estándar
    ///   Observaciones              -> Observations
    ///
    /// El legacy define UNIQUE(CodigoTransportista, CodigoVehiculo) — es
    /// redundante dado que CodigoVehiculo ya es PK por sí solo, así que no se
    /// replica aparte. UNIQUE(Placa) sí es una regla de negocio real y se
    /// mantiene.
    /// </summary>
    public class Vehiculo : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string TransportistaCode { get; set; } = default!;
        public Transportista Transportista { get; set; } = default!;

        public string LicensePlate { get; set; } = default!;

        public string? VehicleCategory { get; set; }
        public string? VehicleType { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public short? ManufactureYear { get; set; }
        public string? EngineNumber { get; set; }
        public string? ChassisNumber { get; set; }
        public string? Color { get; set; }

        public decimal? CargoCapacityKg { get; set; }
        public decimal? LengthM { get; set; }
        public decimal? WidthM { get; set; }
        public decimal? HeightM { get; set; }

        public string? VehicularCertificateNumber { get; set; }
        public string? CirculationCardNumber { get; set; }
        public string? VehicularConfiguration { get; set; }

        public string? Observations { get; set; }

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
