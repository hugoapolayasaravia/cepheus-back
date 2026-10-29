namespace Cepheus.Application.Features.Facturacion.Maestros.Vehiculos.Common
{
    public class VehiculoResponse
    {
        public string TransportistaCode { get; set; } = default!;
        public string VehicleType { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string LicensePlate { get; set; } = default!;
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? ChoferCode { get; set; }
        public decimal Capacity { get; set; }
        public decimal Suple { get; set; }
        public decimal LengthM { get; set; }
        public decimal WidthM { get; set; }
        public decimal HeightM { get; set; }
        public decimal Telescopic { get; set; }
        public decimal WithoutSuple { get; set; }
        public decimal WithSuple { get; set; }
        public decimal CubicWithoutSuple { get; set; }
        public decimal CubicWithSuple { get; set; }
        public string? MtcInternalCode { get; set; }
        public string? VehicularConfiguration { get; set; }
        public string? PlanillaCode { get; set; }
        public string? Observations { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
