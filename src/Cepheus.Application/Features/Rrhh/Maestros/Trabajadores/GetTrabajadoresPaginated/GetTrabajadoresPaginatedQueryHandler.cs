using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.GetTrabajadoresPaginated
{
    public class GetTrabajadoresPaginatedQueryHandler
        : IRequestHandler<GetTrabajadoresPaginatedQuery, PagedResult<TrabajadorResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadoresPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<TrabajadorResponse>> Handle(
            GetTrabajadoresPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Rrhh.Maestros.Trabajadores.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(t =>
                    t.Code.ToLower().Contains(search) ||
                    (t.FirstNames != null && t.FirstNames.ToLower().Contains(search)) ||
                    (t.PaternalSurname != null && t.PaternalSurname.ToLower().Contains(search)) ||
                    (t.MaternalSurname != null && t.MaternalSurname.ToLower().Contains(search)) ||
                    (t.Email != null && t.Email.ToLower().Contains(search)));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(t => t.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(t => new TrabajadorResponse
            {
                Code = t.Code,
                FirstNames = t.FirstNames,
                PaternalSurname = t.PaternalSurname,
                MaternalSurname = t.MaternalSurname,
                SexoCode = t.SexoCode,
                EstadoCivilCode = t.EstadoCivilCode,
                NacionalidadCode = t.NacionalidadCode,
                BirthDate = t.BirthDate,
                BirthUbigeoCode = t.BirthUbigeoCode,
                Email = t.Email,
                Phone = t.Phone,
                MobilePhone = t.MobilePhone,
                PhotoUrl = t.PhotoUrl,
                HasDisability = t.HasDisability,
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