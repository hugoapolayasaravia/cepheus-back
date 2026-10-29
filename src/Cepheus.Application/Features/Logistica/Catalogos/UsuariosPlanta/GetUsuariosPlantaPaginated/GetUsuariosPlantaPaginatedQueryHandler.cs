using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.GetUsuariosPlantaPaginated
{
    public class GetUsuariosPlantaPaginatedQueryHandler
        : IRequestHandler<GetUsuariosPlantaPaginatedQuery, PagedResult<UsuarioPlantaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetUsuariosPlantaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<UsuarioPlantaResponse>> Handle(
            GetUsuariosPlantaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Logistica.Catalogos.UsuariosPlanta.Query().AsNoTracking();

            if (request.UserId.HasValue)
            {
                query = query.Where(a => a.UserId == request.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var plantaCode = request.PlantaCode.Trim().ToUpper();
                query = query.Where(a => a.PlantaCode == plantaCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.Trim();
                query = query.Where(a =>
                    a.User.Email.Contains(term) ||
                    a.User.FirstName.Contains(term) ||
                    a.User.LastName.Contains(term) ||
                    a.PlantaCode.Contains(term) ||
                    a.Planta.Name.Contains(term));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.PlantaCode).ThenBy(a => a.UserId)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(a => new UsuarioPlantaResponse
            {
                UserId = a.UserId,
                UserEmail = a.User.Email,
                UserFullName = a.User.FirstName + " " + a.User.LastName,
                PlantaCode = a.PlantaCode,
                PlantaName = a.Planta.Name,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                CreatedBy = a.CreatedBy,
                UpdatedBy = a.UpdatedBy,
                RowVersion = a.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
