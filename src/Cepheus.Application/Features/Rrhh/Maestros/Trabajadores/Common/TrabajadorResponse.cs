namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.Common
{
    public class TrabajadorResponse
    {
        public string Code { get; set; } = default!;
        public string? FirstNames { get; set; }
        public string? PaternalSurname { get; set; }
        public string? MaternalSurname { get; set; }
        public string? SexoCode { get; set; }
        public string? EstadoCivilCode { get; set; }
        public string? NacionalidadCode { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? BirthUbigeoCode { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? MobilePhone { get; set; }
        public string? PhotoUrl { get; set; }
        public bool HasDisability { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}