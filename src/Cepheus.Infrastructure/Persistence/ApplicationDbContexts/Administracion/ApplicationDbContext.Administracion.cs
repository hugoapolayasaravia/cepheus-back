using Cepheus.Domain.Administracion;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.ApplicationDbContexts;

public partial class ApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RoleUser> RoleUsers => Set<RoleUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<Submodulo> Submodulos => Set<Submodulo>();
    public DbSet<Programa> Programas => Set<Programa>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<PermissionRole> PermissionRoles => Set<PermissionRole>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
}