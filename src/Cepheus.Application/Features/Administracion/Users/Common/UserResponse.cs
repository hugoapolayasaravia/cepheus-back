namespace Cepheus.Application.Features.Administracion.Users.Common
{
    /// <summary>
    /// DTO de salida reutilizado en todos los slices de Users que devuelven
    /// datos del usuario (Create, Update, GetById, GetPaginated).
    /// Nunca incluye PasswordHash.
    /// </summary>
    public class UserResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? TrabajadorCode { get; set; }

        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? TrabajadorTipoDocumento { get; set; }
        public string? TrabajadorNumeroDocumento { get; set; }
        public string? TrabajadorNombre { get; set; }
        public bool? InvitationSent { get; set; }

        /// <summary>
        /// Necesario para el body de UpdateUserCommand (control de concurrencia).
        /// System.Text.Json lo serializa/deserializa automáticamente como string Base64.
        /// </summary>
        public byte[] RowVersion { get; set; } = default!;
    }



}
