using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.GetUsersPaginated
{

    public class GetUsersPaginatedQueryHandler
   : IRequestHandler<GetUsersPaginatedQuery, PagedResult<UserResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetUsersPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<UserResponse>> Handle(
            GetUsersPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Administracion.Users.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(search) ||
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            // Defaults resueltos acá (no en la clase PagedRequest): PageNumber/PageSize/SortDesc
            // llegan nullable porque [AsParameters] los trata como obligatorios si no son nullable.
            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            // Ordenamiento dinámico (?sortBy=FirstName&sortDesc=true); si no viene,
            // se usa LastName+FirstName como default para que el listado sea determinístico.
            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                CreatedBy = u.CreatedBy,
                UpdatedBy = u.UpdatedBy,
                TrabajadorCode = u.TrabajadorCode,
                RowVersion = u.RowVersion,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }






}



