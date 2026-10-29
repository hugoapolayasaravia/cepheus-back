namespace Cepheus.Application.Features.Administracion.Roles.Common
{
    public class RoleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Necesario para el body de UpdateRoleCommand (control de concurrencia).
        /// </summary>
        public byte[] RowVersion { get; set; } = default!;
    }

}
