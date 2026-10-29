using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.GetSolicitudesNotaCreditoPaginated
{
    public class GetSolicitudesNotaCreditoPaginatedQueryHandler
        : IRequestHandler<GetSolicitudesNotaCreditoPaginatedQuery, PagedResult<SolicitudNotaCreditoListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetSolicitudesNotaCreditoPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<SolicitudNotaCreditoListItemResponse>> Handle(
            GetSolicitudesNotaCreditoPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Transacciones.SolicitudesNotaCredito.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.NegocioCode))
            {
                var negocio = SolicitudNotaCreditoRules.Normalize(request.NegocioCode);
                query = query.Where(s => s.NegocioCode == negocio);
            }

            if (!string.IsNullOrWhiteSpace(request.ClienteCode))
            {
                var cliente = request.ClienteCode.Trim();
                query = query.Where(s => s.ClienteCode == cliente);
            }

            if (!string.IsNullOrWhiteSpace(request.Year))
            {
                var year = request.Year.Trim();
                query = query.Where(s => s.Year == year);
            }

            if (!string.IsNullOrWhiteSpace(request.Month))
            {
                var month = request.Month.Trim().PadLeft(2, '0');
                query = query.Where(s => s.Month == month);
            }

            if (request.DateFrom.HasValue)
            {
                var from = request.DateFrom.Value.Date;
                query = query.Where(s => s.IssueDate >= from);
            }

            if (request.DateTo.HasValue)
            {
                var to = request.DateTo.Value.Date.AddDays(1);
                query = query.Where(s => s.IssueDate < to);
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!System.Enum.TryParse<EstadoSolicitudNotaCredito>(request.Status.Trim(), true, out var status)
                    || !System.Enum.IsDefined(typeof(EstadoSolicitudNotaCredito), status))
                {
                    throw new InvalidOperationException(
                        $"El estado debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(EstadoSolicitudNotaCredito)))}.");
                }

                query = query.Where(s => s.Status == status);
            }

            if (request.OnlyApproved == true)
            {
                query = query.Where(s => s.Status == EstadoSolicitudNotaCredito.Pendiente
                    && s.Approvals.Any()
                    && !s.Approvals.Any(a => a.Status != EstadoAprobacionSolicitud.Aprobado));
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(s =>
                    s.Code.ToLower().Contains(search) ||
                    s.DocumentNumber.ToLower().Contains(search) ||
                    s.Observation.ToLower().Contains(search) ||
                    (s.Cliente.Name != null && s.Cliente.Name.ToLower().Contains(search)));
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(s => s.IssueDate).ThenByDescending(s => s.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(SolicitudNotaCreditoMapper.ToListItem);

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
