using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.GetTecnicosPaginated
{
    public class GetTecnicosPaginatedQueryHandler
        : IRequestHandler<GetTecnicosPaginatedQuery, PagedResult<TecnicoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTecnicosPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<TecnicoResponse>> Handle(
            GetTecnicosPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Maestros.Tecnicos.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.TrabajadorCode.ToLower().Contains(search) ||
                    (t.Trabajador.FirstNames ?? "").ToLower().Contains(search) ||
                    (t.Trabajador.PaternalSurname ?? "").ToLower().Contains(search));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.TrabajadorCode)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(t => new TecnicoResponse
            {
                TrabajadorCode = t.TrabajadorCode,
                TrabajadorNombre = t.Trabajador.FirstNames + " " + t.Trabajador.PaternalSurname,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CreatedBy = t.CreatedBy,
                UpdatedBy = t.UpdatedBy,
                RowVersion = t.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
