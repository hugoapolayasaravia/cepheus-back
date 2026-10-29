using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Administracion;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Cepheus.Infrastructure.Persistence.Repositories;

namespace Cepheus.Infrastructure.Persistence.UnitOfWorks.Administracion;

public sealed class AdministracionUnitOfWork : IAdministracionUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public AdministracionUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    private IRepository<User>? _users;
    public IRepository<User> Users => _users ??= new Repository<User>(_context);

    private IRepository<Role>? _roles;
    public IRepository<Role> Roles =>_roles ??= new Repository<Role>(_context);

    private IRepository<RoleUser>? _roleUsers;
    public IRepository<RoleUser> RoleUsers => _roleUsers ??= new Repository<RoleUser>(_context);

    private IRepository<RefreshToken>? _refreshTokens;
    public IRepository<RefreshToken> RefreshTokens => _refreshTokens ??= new Repository<RefreshToken>(_context);

    private IRepository<Modulo>? _modulos;
    public IRepository<Modulo> Modulos => _modulos ??= new Repository<Modulo>(_context);

    private IRepository<Submodulo>? _submodulos;
    public IRepository<Submodulo> Submodulos => _submodulos ??= new Repository<Submodulo>(_context);

    private IRepository<Programa>? _programas;
    public IRepository<Programa> Programas => _programas ??= new Repository<Programa>(_context);

    private IRepository<Permission>? _permissions;
    public IRepository<Permission> Permissions => _permissions ??= new Repository<Permission>(_context);

    private IRepository<PermissionRole>? _permissionRoles;
    public IRepository<PermissionRole> PermissionRoles => _permissionRoles ??= new Repository<PermissionRole>(_context);

    private IRepository<PasswordResetToken>? _passwordResetTokens;
    public IRepository<PasswordResetToken> PasswordResetTokens => _passwordResetTokens ??= new Repository<PasswordResetToken>(_context);
}