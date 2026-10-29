using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.GetActividadesPaginated
{
    public class GetActividadesPaginatedQueryHandler
        : IRequestHandler<GetActividadesPaginatedQuery, PagedResult<ActividadResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetActividadesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ActividadResponse>> Handle(
            GetActividadesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Mantenimiento.Maestros.Actividades.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(a =>
                    a.Code.ToLower().Contains(search) ||
                    a.VerboActividad.Name.ToLower().Contains(search) ||
                    a.ObjetoActividad.Name.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.VerboActividadCode))
            {
                var verboCode = request.VerboActividadCode.Trim().ToUpper();
                query = query.Where(a => a.VerboActividadCode == verboCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ObjetoActividadCode))
            {
                var objetoCode = request.ObjetoActividadCode.Trim().ToUpper();
                query = query.Where(a => a.ObjetoActividadCode == objetoCode);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(a => a.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(a => new ActividadResponse
            {
                Code = a.Code,
                VerboActividadCode = a.VerboActividadCode,
                VerboActividadName = a.VerboActividad.Name,
                ObjetoActividadCode = a.ObjetoActividadCode,
                ObjetoActividadName = a.ObjetoActividad.Name,
                IsActive = a.IsActive,
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
