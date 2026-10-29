using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GetChequesVentaPaginated
{
    public class GetChequesVentaPaginatedQueryHandler
        : IRequestHandler<GetChequesVentaPaginatedQuery, PagedResult<ChequeVentaListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetChequesVentaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ChequeVentaListItemResponse>> Handle(
            GetChequesVentaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Transacciones.ChequesVenta.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Number))
            {
                var number = request.Number.Trim();
                query = query.Where(c => c.Number.Contains(number));
            }
            else if (!string.IsNullOrWhiteSpace(request.CancelacionCode))
            {
                var can = request.CancelacionCode.Trim();
                query = query.Where(c => c.CancelacionCode == can);
            }
            else
            {
                if (request.DateFrom.HasValue)
                {
                    var from = request.DateFrom.Value.Date;
                    query = query.Where(c => c.IssueDate >= from);
                }

                if (request.DateTo.HasValue)
                {
                    var to = request.DateTo.Value.Date.AddDays(1);
                    query = query.Where(c => c.IssueDate < to);
                }

                if (!string.IsNullOrWhiteSpace(request.ClienteCode))
                {
                    var cliente = request.ClienteCode.Trim();
                    query = query.Where(c => c.ClienteCode == cliente);
                }

                if (!string.IsNullOrWhiteSpace(request.BancoCode))
                {
                    var banco = ChequeVentaRules.Normalize(request.BancoCode);
                    query = query.Where(c => c.BancoCode == banco);
                }

                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    if (!System.Enum.TryParse<EstadoCheque>(request.Status.Trim(), true, out var status)
                        || !System.Enum.IsDefined(typeof(EstadoCheque), status))
                    {
                        throw new InvalidOperationException(
                            $"El estado debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(EstadoCheque)))}.");
                    }

                    query = query.Where(c => c.Status == status);
                }
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(c => c.IssueDate).ThenByDescending(c => c.Number)
                : query.ApplySort(request.SortBy, sortDesc);

            return await sorted.Select(ChequeVentaMapper.ToListItem)
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
