using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.GetObrasPaginated
{
    public class GetObrasPaginatedQueryHandler
        : IRequestHandler<GetObrasPaginatedQuery, PagedResult<ObraResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetObrasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ObraResponse>> Handle(
            GetObrasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Maestros.Obras.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(o =>
                    o.Code.ToLower().Contains(search) ||
                    o.Description.ToLower().Contains(search) ||
                    o.ShortName.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.ClienteCode))
            {
                var clienteCode = request.ClienteCode.Trim().ToUpper();
                query = query.Where(o => o.ClienteCode == clienteCode);
            }

            if (!string.IsNullOrWhiteSpace(request.VendedorCode))
            {
                var vendedorCode = request.VendedorCode.Trim().ToUpper();
                query = query.Where(o => o.VendedorCode == vendedorCode);
            }

            if (!string.IsNullOrWhiteSpace(request.CobradorCode))
            {
                var cobradorCode = request.CobradorCode.Trim().ToUpper();
                query = query.Where(o => o.CobradorCode == cobradorCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado) &&
                System.Enum.TryParse<EstadoObra>(request.Estado, true, out var estado))
            {
                query = query.Where(o => o.Estado == estado);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(o => o.ClienteCode).ThenBy(o => o.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var obras = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<ObraResponse>
            {
                Items = obras.Items.Select(Cepheus.Application.Features.Facturacion.Maestros.Obras.CreateObra.CreateObraCommandHandler.Map).ToList(),
                TotalCount = obras.TotalCount,
                PageNumber = obras.PageNumber,
                PageSize = obras.PageSize
            };
        }
    }
}
