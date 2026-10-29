namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.Common
{
    public class TrabajadorDomicilioResponse
    {
        public int Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string? RoadTypeCode { get; set; }
        public string? StreetName { get; set; }
        public string? StreetNumber { get; set; }
        public string? InteriorNumber { get; set; }
        public string? ZoneTypeCode { get; set; }
        public string? ZoneName { get; set; }
        public string? Reference { get; set; }
        public string? UbigeoCode { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}