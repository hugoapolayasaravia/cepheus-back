using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Users.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserResponse>
    {
        private const string BootstrapRoleName = "Administrador";
        private const string BootstrapRoleDescription = "Administrador del sistema";

        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterCommandHandler(IUnitOfWork uow, IPasswordHasher passwordHasher)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Bootstrap: solo se permite si NO existe ningún usuario todavía.
            var anyUserExists = await _uow.Administracion.Users.Query().AnyAsync(cancellationToken);
            if (anyUserExists)
            {
                throw new InvalidOperationException(
                    "El registro inicial ya fue completado. Solicite a un administrador que le cree una cuenta.");
            }

            var role = await _uow.Administracion.Roles.Query()
                .FirstOrDefaultAsync(r => r.Name == BootstrapRoleName, cancellationToken);

            if (role is null)
            {
                role = new Role
                {
                    Name = BootstrapRoleName,
                    Description = BootstrapRoleDescription,
                    IsActive = true
                };
                await _uow.Administracion.Roles.AddAsync(role, cancellationToken);
                await _uow.SaveChangesAsync(cancellationToken); // Necesita Id antes de asociarlo en RoleUser
            }

            var user = new User
            {
                Email = request.Email.Trim().ToLowerInvariant(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                IsActive = true
            };

            await _uow.Administracion.Users.AddAsync(user, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken); // Necesita Id antes de asociarlo en RoleUser

            await _uow.Administracion.RoleUsers.AddAsync(new RoleUser
            {
                UserId = user.Id,
                RoleId = role.Id
            }, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                RowVersion = user.RowVersion,
                Roles = new List<string> { role.Name }
            };
        }
    }



}
