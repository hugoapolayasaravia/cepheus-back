using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.CreateCuentaBancaria;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.GetCuentasBancariasPaginated
{
    public class GetCuentasBancariasPaginatedQueryHandler
        : IRequestHandler<GetCuentasBancariasPaginatedQuery, PagedResult<CuentaBancariaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCuentasBancariasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CuentaBancariaResponse>> Handle(
            GetCuentasBancariasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Maestros.CuentasBancarias.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(x =>
                    x.AccountNumber.ToLower().Contains(search) ||
                    x.BankCode.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.BankCode))
            {
                var bank = request.BankCode.Trim();
                query = query.Where(x => x.BankCode == bank);
            }

            if (!string.IsNullOrWhiteSpace(request.CurrencyCode))
            {
                var currency = request.CurrencyCode.Trim().ToUpper();
                query = query.Where(x => x.CurrencyCode == currency);
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == request.IsActive.Value);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(x => x.BankCode).ThenBy(x => x.AccountNumber)
                : query.ApplySort(request.SortBy, sortDesc);

            // Se pagina la entidad y se mapea en memoria (mismo criterio que OrdenTrabajo).
            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<CuentaBancariaResponse>
            {
                Items = paged.Items.Select(CreateCuentaBancariaCommandHandler.Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
