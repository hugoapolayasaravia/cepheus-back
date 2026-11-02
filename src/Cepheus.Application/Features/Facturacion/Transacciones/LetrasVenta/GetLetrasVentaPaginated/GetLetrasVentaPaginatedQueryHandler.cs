using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetrasVentaPaginated
{
    public class GetLetrasVentaPaginatedQueryHandler
        : IRequestHandler<GetLetrasVentaPaginatedQuery, PagedResult<LetraVentaListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetLetrasVentaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<LetraVentaListItemResponse>> Handle(
            GetLetrasVentaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Transacciones.LetrasVenta.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Number))
            {
                var number = request.Number.Trim();
                query = query.Where(l => l.Number == number);
            }
            else if (!string.IsNullOrWhiteSpace(request.CancelacionCode))
            {
                var can = request.CancelacionCode.Trim();
                query = query.Where(l => l.CancelacionCode == can);
            }
            else
            {
                if (request.DateFrom.HasValue)
                {
                    var from = request.DateFrom.Value.Date;
                    query = query.Where(l => l.IssueDate >= from);
                }

                if (request.DateTo.HasValue)
                {
                    var to = request.DateTo.Value.Date.AddDays(1);
                    query = query.Where(l => l.IssueDate < to);
                }

                if (!string.IsNullOrWhiteSpace(request.ClienteCode))
                {
                    var cliente = request.ClienteCode.Trim();
                    query = query.Where(l => l.ClienteCode == cliente);
                }

                if (!string.IsNullOrWhiteSpace(request.CobradorCode))
                {
                    var cobrador = request.CobradorCode.Trim();
                    query = query.Where(l => l.CobradorCode == cobrador);
                }

                if (!string.IsNullOrWhiteSpace(request.BancoCode))
                {
                    var banco = LetraVentaRules.Normalize(request.BancoCode);
                    query = query.Where(l => l.BancoCode == banco);
                }

                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    if (!System.Enum.TryParse<EstadoLetra>(request.Status.Trim(), true, out var status)
                        || !System.Enum.IsDefined(typeof(EstadoLetra), status))
                    {
                        throw new InvalidOperationException(
                            $"El estado debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(EstadoLetra)))}.");
                    }

                    query = query.Where(l => l.Status == status);
                }
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(l => l.IssueDate).ThenByDescending(l => l.Number)
                : query.ApplySort(request.SortBy, sortDesc);

            return await sorted.Select(LetraVentaMapper.ToListItem)
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
