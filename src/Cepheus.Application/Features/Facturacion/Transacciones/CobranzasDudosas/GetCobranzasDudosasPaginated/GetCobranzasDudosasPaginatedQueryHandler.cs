using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.GetCobranzasDudosasPaginated
{
    public class GetCobranzasDudosasPaginatedQueryHandler
        : IRequestHandler<GetCobranzasDudosasPaginatedQuery, PagedResult<CobranzaDudosaResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCobranzasDudosasPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CobranzaDudosaResponse>> Handle(
            GetCobranzasDudosasPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Transacciones.CobranzasDudosas.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.TypeCode))
            {
                var type = request.TypeCode.Trim();
                query = query.Where(x => x.TypeCode == type);
            }

            if (!string.IsNullOrWhiteSpace(request.Number))
            {
                var number = request.Number.Trim();
                query = query.Where(x => x.Number == number);
            }

            if (!string.IsNullOrWhiteSpace(request.ClienteCode))
            {
                var cliente = request.ClienteCode.Trim();
                query = query.Where(x => x.ClienteCode == cliente);
            }

            if (request.DateFrom.HasValue)
            {
                var from = request.DateFrom.Value.Date;
                query = query.Where(x => x.ProcessDate >= from);
            }

            if (request.DateTo.HasValue)
            {
                var to = request.DateTo.Value.Date;
                query = query.Where(x => x.ProcessDate <= to);
            }

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!System.Enum.TryParse<EstadoRegistroCobranzaDudosa>(request.Status.Trim(), true, out var status)
                    || !System.Enum.IsDefined(typeof(EstadoRegistroCobranzaDudosa), status))
                {
                    throw new InvalidOperationException(
                        $"El estado debe ser uno de: {string.Join(", ", System.Enum.GetNames(typeof(EstadoRegistroCobranzaDudosa)))}.");
                }

                query = query.Where(x => x.Status == status);
            }

            if (request.OnlyWithoutCanje == true)
            {
                query = query.Where(x => x.CanjeEntry == null || x.CanjeEntry == "");
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sorted = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(x => x.ProcessDate).ThenBy(x => x.TypeCode).ThenBy(x => x.Number)
                : query.ApplySort(request.SortBy, sortDesc);

            var page = await sorted.Select(x => new CobranzaDudosaResponse
                {
                    TypeCode = x.TypeCode,
                    PlantaCode = x.PlantaCode,
                    Number = x.Number,
                    ClienteCode = x.ClienteCode,
                    BancoCode = x.BancoCode,
                    ObraCode = x.ObraCode,
                    ProcessDate = x.ProcessDate,
                    SystemDate = x.SystemDate,
                    TipoContabilidad = x.TipoContabilidad,
                    CurrencyCode = x.CurrencyCode,
                    ExchangeRate = x.ExchangeRate,
                    TotalSoles = x.TotalSoles,
                    TotalDollars = x.TotalDollars,
                    ProvisionEntry = x.ProvisionEntry,
                    CanjeEntry = x.CanjeEntry,
                    Status = x.Status,
                    WrittenOffAt = x.WrittenOffAt,
                    RowVersion = x.RowVersion
                })
                .ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            await CobranzaDudosaMapper.AttachClienteNamesAsync(_uow, page.Items, cancellationToken);

            return page;
        }
    }
}
