namespace Cepheus.Domain.Administracion
{
    /// <summary>
    /// Tabla de unión roles_x_users. PK compuesta (UserId + RoleId),
    /// configurada en RoleUserConfiguration (Infrastructure).
    /// </summary>
    public class RoleUser
    {
        public int UserId { get; set; }
        public User User { get; set; } = default!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = default!;
    }

}
