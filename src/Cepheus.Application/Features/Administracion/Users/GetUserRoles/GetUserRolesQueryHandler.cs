using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Roles.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.GetUserRoles
{
    public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<RoleResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetUserRolesQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<RoleResponse>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var userExists = await _uow.Administracion.Users.Query()
                .AnyAsync(u => u.Id == request.UserId, cancellationToken);

            if (!userExists)
            {
                throw new KeyNotFoundException($"Usuario {request.UserId} no encontrado.");
            }

            return await _uow.Administracion.RoleUsers.Query()
                .Where(ru => ru.UserId == request.UserId)
                .Select(ru => new RoleResponse
                {
                    Id = ru.Role.Id,
                    Name = ru.Role.Name,
                    Description = ru.Role.Description,
                    IsActive = ru.Role.IsActive,
                    CreatedAt = ru.Role.CreatedAt,
                    UpdatedAt = ru.Role.UpdatedAt,
                    RowVersion = ru.Role.RowVersion
                })
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
    }

}
