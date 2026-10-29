using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.GetClientesPaginated
{
    public class GetClientesPaginatedQueryHandler
        : IRequestHandler<GetClientesPaginatedQuery, PagedResult<ClienteResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetClientesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ClienteResponse>> Handle(
            GetClientesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Maestros.Clientes.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.DocumentNumber.ToLower().Contains(search) ||
                    (c.Name != null && c.Name.ToLower().Contains(search)) ||
                    c.ContactName.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.TipoClienteCode))
            {
                var tipoClienteCode = request.TipoClienteCode.Trim().ToUpper();
                query = query.Where(c => c.TipoClienteCode == tipoClienteCode);
            }

            if (!string.IsNullOrWhiteSpace(request.ClasificacionClienteCode))
            {
                var clasificacionCode = request.ClasificacionClienteCode.Trim().ToUpper();
                query = query.Where(c => c.ClasificacionClienteCode == clasificacionCode);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado) &&
                System.Enum.TryParse<EstadoCliente>(request.Estado, true, out var estado))
            {
                query = query.Where(c => c.Estado == estado);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(c => c.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var clientes = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<ClienteResponse>
            {
                Items = clientes.Items.Select(Cepheus.Application.Features.Facturacion.Maestros.Clientes.CreateCliente.CreateClienteCommandHandler.Map).ToList(),
                TotalCount = clientes.TotalCount,
                PageNumber = clientes.PageNumber,
                PageSize = clientes.PageSize
            };
        }
    }
}
