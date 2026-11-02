using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GetCancelacionesVentaPaginated
{
    public class GetCancelacionesVentaPaginatedQueryHandler
        : IRequestHandler<GetCancelacionesVentaPaginatedQuery, PagedResult<CancelacionVentaListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCancelacionesVentaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CancelacionVentaListItemResponse>> Handle(
            GetCancelacionesVentaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Transacciones.CancelacionesVenta.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim();
                query = query.Where(c => c.Code == code);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(request.ClienteCode))
                {
                    var cliente = request.ClienteCode.Trim();
                    query = query.Where(c => c.ClienteCode == cliente);
                }

                if (!string.IsNullOrWhiteSpace(request.PlantaCode))
                {
                    var planta = LetraVentaRules.Normalize(request.PlantaCode);
                    query = query.Where(c => c.PlantaCode == planta);
                }

                if (!string.IsNullOrWhiteSpace(request.TipoPagoCode))
                {
                    var tipoPago = request.TipoPagoCode.Trim();
                    query = query.Where(c => c.TipoPagoCode == tipoPago);
                }

                if (request.DateFrom.HasValue)
                {
                    var from = request.DateFrom.Value.Date;
                    query = query.Where(c => c.Date >= from);
                }

                if (request.DateTo.HasValue)
                {
                    var to = request.DateTo.Value.Date.AddDays(1);
                    query = query.Where(c => c.Date < to);
                }

                if (!string.IsNullOrWhiteSpace(request.Status))
                {
                    if (!System.Enum.TryParse<EstadoCancelacion>(request.Status.Trim(), true, out var status)
                        || !System.Enum.IsDefined(typeof(EstadoCancelacion), status))
                    {
                        throw new InvalidOperationException(
                            $"El estado debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(EstadoCancelacion)))}.");
                    }

                    query = query.Where(c => c.Status == status);
                }
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(c => c.Date).ThenByDescending(c => c.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            return await sorted.Select(CancelacionVentaMapper.ToListItem)
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
