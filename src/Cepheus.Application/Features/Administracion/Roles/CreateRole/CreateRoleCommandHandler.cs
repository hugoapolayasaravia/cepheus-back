using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Roles.Common;
using Cepheus.Domain.Administracion;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateRoleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RoleResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new Role
            {
                Name = request.Name.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                IsActive = true
            };

            await _uow.Administracion.Roles.AddAsync(role, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

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
