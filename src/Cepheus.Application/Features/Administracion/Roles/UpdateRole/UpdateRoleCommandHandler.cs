using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Roles.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Roles.UpdateRole
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateRoleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RoleResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Administracion.Roles.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Rol {request.Id} no encontrado.");
            }

            var role = new Role
            {
                Id = request.Id,
                Name = request.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),

                // Campos que este Update NO modifica: se conservan del registro actual
                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Administracion.Roles.Update(role);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El rol fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                CreatedAt = role.CreatedAt,
                UpdatedAt = role.UpdatedAt,
                RowVersion = role.RowVersion
            };
        }
    }

}
