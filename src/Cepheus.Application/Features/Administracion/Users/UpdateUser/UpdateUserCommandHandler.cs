using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Users.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateUserCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Administracion.Users.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Usuario {request.Id} no encontrado.");
            }

            var user = new User
            {
                Id = request.Id,
                Email = request.Email.Trim().ToLowerInvariant(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                TrabajadorCode = request.TrabajadorCode,
                // Campos que este Update NO modifica: se conservan del registro actual
                PasswordHash = current.PasswordHash,
                
                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Administracion.Users.Update(user);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El usuario fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TrabajadorCode = user.TrabajadorCode!,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                RowVersion = user.RowVersion
            };
        }
    }



}
