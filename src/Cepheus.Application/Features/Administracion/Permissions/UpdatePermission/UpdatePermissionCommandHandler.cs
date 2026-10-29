using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Permissions.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Permissions.UpdatePermission
{
    public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, PermissionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePermissionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PermissionResponse> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Administracion.Permissions.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Permiso {request.Id} no encontrado.");
            }

            var permission = new Permission
            {
                Id = request.Id,
                ProgramaId = current.ProgramaId, // no editable acá
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Administracion.Permissions.Update(permission);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El permiso fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new PermissionResponse
            {
                Id = permission.Id,
                ProgramaId = permission.ProgramaId,
                Code = permission.Code,
                Name = permission.Name,
                IsActive = permission.IsActive,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt,
                CreatedBy = permission.CreatedBy,
                UpdatedBy = permission.UpdatedBy,
                RowVersion = permission.RowVersion
            };
        }
    }

}
