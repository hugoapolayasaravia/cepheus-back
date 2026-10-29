using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.GetNotasCreditoVentaPaginated
{
    public class GetNotasCreditoVentaPaginatedQueryHandler
        : IRequestHandler<GetNotasCreditoVentaPaginatedQuery, PagedResult<NotaCreditoVentaListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetNotasCreditoVentaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<NotaCreditoVentaListItemResponse>> Handle(
            GetNotasCreditoVentaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Transacciones.NotasCreditoVenta.Query().AsNoTracking();

            if (request.DateFrom.HasValue)
            {
                var from = request.DateFrom.Value.Date;
                query = query.Where(n => n.IssueDate >= from);
            }

            if (request.DateTo.HasValue)
            {
                var to = request.DateTo.Value.Date.AddDays(1);
                query = query.Where(n => n.IssueDate < to);
            }

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var planta = NotaCreditoVentaRules.Normalize(request.PlantaCode);
                query = query.Where(n => n.PlantaCode == planta);
            }

            if (!string.IsNullOrWhiteSpace(request.ClienteCode))
            {
                var cliente = request.ClienteCode.Trim();
                query = query.Where(n => n.ClienteCode == cliente);
            }

            if (!string.IsNullOrWhiteSpace(request.Number))
            {
                var numero = request.Number.Trim();
                query = query.Where(n => n.Number.Contains(numero));
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!System.Enum.TryParse<EstadoNotaCredito>(request.Status.Trim(), true, out var status)
                    || !System.Enum.IsDefined(typeof(EstadoNotaCredito), status))
                {
                    throw new InvalidOperationException(
                        $"El estado debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(EstadoNotaCredito)))}.");
                }

                query = query.Where(n => n.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(n =>
                    n.Number.ToLower().Contains(search) ||
                    n.DocumentNumber.ToLower().Contains(search) ||
                    n.Description.ToLower().Contains(search) ||
                    (n.Cliente.Name != null && n.Cliente.Name.ToLower().Contains(search)));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(n => n.IssueDate).ThenByDescending(n => n.Number)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(NotaCreditoVentaMapper.ToListItem);

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
