using System.Security.Cryptography;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Users.Common;
using Cepheus.Domain.Administracion;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserInvitationService _invitationService;

        public CreateUserCommandHandler(
            IUnitOfWork uow,
            IPasswordHasher passwordHasher,
            IUserInvitationService invitationService)
        {
            _uow = uow;
            _passwordHasher = passwordHasher;
            _invitationService = invitationService;
        }

        public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // La unicidad de Username/Email ya fue validada en CreateUserCommandValidator
            // (pipeline de MediatR corre antes que el handler). Acá solo se arma la entidad.

            var enInvitacion = string.IsNullOrEmpty(request.Password);

            // En modo invitación la clave inicial es aleatoria y nadie la conoce: el usuario define
            // la suya con el enlace del correo.
            var clave = enInvitacion
                ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                : request.Password!;

            var user = new User
            {
                Email = request.Email.Trim().ToLowerInvariant(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                PasswordHash = _passwordHasher.Hash(clave),
                IsActive = true
            };

            await _uow.Administracion.Users.AddAsync(user, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            bool? invitacionEnviada = null;
            if (enInvitacion)
            {
                invitacionEnviada = await _invitationService.SendAsync(user, cancellationToken);
            }

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
                InvitationSent = invitacionEnviada
            };
        }
    }


}
