using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.GetCajasChicaPaginated
{
    public class GetCajasChicaPaginatedQueryHandler
        : IRequestHandler<GetCajasChicaPaginatedQuery, PagedResult<CajaChicaListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCajasChicaPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CajaChicaListItemResponse>> Handle(
            GetCajasChicaPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Contabilidad.Transacciones.CajasChica.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim();
                query = query.Where(c => c.Code.Contains(code));
            }

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

            if (!string.IsNullOrWhiteSpace(request.PlantaCode))
            {
                var planta = CajaChicaRules.Normalize(request.PlantaCode);
                query = query.Where(c => c.PlantaCode == planta);
            }

            if (!string.IsNullOrWhiteSpace(request.TrabajadorCode))
            {
                var trabajador = request.TrabajadorCode.Trim();
                query = query.Where(c => c.TrabajadorCode == trabajador);
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!System.Enum.TryParse<EstadoCajaChica>(request.Status.Trim(), true, out var status)
                    || !System.Enum.IsDefined(typeof(EstadoCajaChica), status))
                {
                    throw new InvalidOperationException(
                        $"El estado debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(EstadoCajaChica)))}.");
                }

                query = query.Where(c => c.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(request.Type))
            {
                var tipo = CajaChicaRules.ParseTipo(request.Type);
                query = query.Where(c => c.Type == tipo);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(c => c.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(CajaChicaMapper.ToListItem);

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
