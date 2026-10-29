namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.Common
{
    public class TrabajadorJornadaResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string? HorarioCode { get; set; }
        public bool HorasExtras { get; set; }
        public bool HorasExt40 { get; set; }
        public bool HorasExtCon { get; set; }
        public decimal HorasExtCon125 { get; set; }
        public decimal HorasExtCon135 { get; set; }
        public bool ControlHorario { get; set; }
        public bool HorarioOrdinario { get; set; }
        public bool HorarioNocturno { get; set; }
        public bool JornadaMaxima { get; set; }
        public bool RegimenAlternativo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
