using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Administracion
{
    public class User : IAuditableEntity
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? TrabajadorCode { get; set; }
        public Trabajador? Trabajador { get; set; }
        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;

        // Navegación
        public ICollection<RoleUser> UserRoles { get; set; } = new List<RoleUser>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }



}
