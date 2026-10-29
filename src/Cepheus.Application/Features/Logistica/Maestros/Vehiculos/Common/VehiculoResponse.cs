namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.Common
{
    public class VehiculoResponse
    {
        public string Code { get; set; } = default!;
        public string TransportistaCode { get; set; } = default!;
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
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
