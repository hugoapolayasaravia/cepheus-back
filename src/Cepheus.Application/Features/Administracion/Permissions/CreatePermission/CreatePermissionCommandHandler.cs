using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Permissions.Common;
using Cepheus.Domain.Administracion;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Permissions.CreatePermission
{
    public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, PermissionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePermissionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PermissionResponse> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = new Permission
            {
                ProgramaId = request.ProgramaId,
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Administracion.Permissions.AddAsync(permission, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

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
