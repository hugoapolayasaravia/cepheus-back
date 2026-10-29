using Cepheus.Domain.Administracion;

namespace Cepheus.Application.Comun.Interfaces.UnitOfWork
{
    public interface IAdministracionUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<Role> Roles { get; }
        IRepository<RoleUser> RoleUsers { get; }
        IRepository<RefreshToken> RefreshTokens { get; }

        IRepository<Modulo> Modulos { get; }
        IRepository<Submodulo> Submodulos { get; }
        IRepository<Programa> Programas { get; }

        IRepository<Permission> Permissions { get; }
        IRepository<PermissionRole> PermissionRoles { get; }
        IRepository<PasswordResetToken> PasswordResetTokens { get; }
    }
}
