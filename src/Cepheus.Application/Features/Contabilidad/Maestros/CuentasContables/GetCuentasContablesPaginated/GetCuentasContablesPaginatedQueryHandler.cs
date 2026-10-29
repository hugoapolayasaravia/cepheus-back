using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.CreateCuentaContable;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasContables.GetCuentasContablesPaginated
{
    public class GetCuentasContablesPaginatedQueryHandler
        : IRequestHandler<GetCuentasContablesPaginatedQuery, PagedResult<CuentaContableResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCuentasContablesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CuentaContableResponse>> Handle(
            GetCuentasContablesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Maestros.CuentasContables.Query().AsNoTracking();

            // Mismos parámetros que sp_con_mira_cuentas (@digitos, @codigo, @nombre). A
            // diferencia del legacy, que usaba solo uno de ellos, aquí se combinan todos.
            if (request.Digits is > 0)
            {
                var digits = request.Digits.Value;
                query = query.Where(x => x.Code.Length == digits);
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim();
                query = query.Where(x => x.Code.StartsWith(code));
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var name = request.Name.Trim().ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(name));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            // Se pagina la entidad y se mapea en memoria (mismo criterio que OrdenTrabajo).
            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<CuentaContableResponse>
            {
                Items = paged.Items.Select(CreateCuentaContableCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
