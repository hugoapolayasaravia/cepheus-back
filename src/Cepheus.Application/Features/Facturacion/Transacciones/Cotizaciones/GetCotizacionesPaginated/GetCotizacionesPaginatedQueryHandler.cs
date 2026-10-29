using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.GetCotizacionesPaginated
{
    public class GetCotizacionesPaginatedQueryHandler
        : IRequestHandler<GetCotizacionesPaginatedQuery, PagedResult<CotizacionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetCotizacionesPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<CotizacionResponse>> Handle(
            GetCotizacionesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = _uow.Facturacion.Transacciones.Cotizaciones.Query().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.NegocioCode))
                query = query.Where(c => c.NegocioCode == request.NegocioCode.Trim().ToUpper());

            if (!string.IsNullOrWhiteSpace(request.Year))
                query = query.Where(c => c.Year == request.Year);

            if (!string.IsNullOrWhiteSpace(request.VendedorCode))
                query = query.Where(c => c.VendedorCode == request.VendedorCode.Trim().ToUpper());

            if (!string.IsNullOrWhiteSpace(request.ClienteCode))
                query = query.Where(c => c.ClienteCode == request.ClienteCode.Trim().ToUpper());

            if (!string.IsNullOrWhiteSpace(request.Status)
                && System.Enum.TryParse<EstadoCotizacion>(request.Status, true, out var status))
                query = query.Where(c => c.Status == status);

            if (request.DateFrom.HasValue)
                query = query.Where(c => c.Date >= request.DateFrom.Value);

            if (request.DateTo.HasValue)
                query = query.Where(c => c.Date <= request.DateTo.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(search) ||
                    c.ClientName.ToLower().Contains(search) ||
                    c.WorkName.ToLower().Contains(search));
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(c => c.Date).ThenByDescending(c => c.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var projected = sortedQuery.Select(c => new CotizacionResponse
            {
                NegocioCode = c.NegocioCode,
                Year = c.Year,
                Month = c.Month,
                Code = c.Code,
                VendedorCode = c.VendedorCode,
                Date = c.Date,
                CurrencyCode = c.CurrencyCode,
                FormaPagoVentaCode = c.FormaPagoVentaCode,
                TecnicoCode = c.TecnicoCode,
                AppliesIgv = c.AppliesIgv,
                Discount = c.Discount,
                GlobalVolume = c.GlobalVolume,
                IsEditable = c.IsEditable,
                Type = c.Type.ToString(),
                MetradoCalculationSystem = c.MetradoCalculationSystem == null ? null : c.MetradoCalculationSystem.ToString(),
                WorkDurationMonths = c.WorkDurationMonths,
                ClienteCode = c.ClienteCode,
                Ruc = c.Ruc,
                ClientName = c.ClientName,
                ClientAddress = c.ClientAddress,
                ClientAddressUbigeoCode = c.ClientAddressUbigeoCode,
                ObraCode = c.ObraCode,
                WorkName = c.WorkName,
                ProjectStatus = c.ProjectStatus.ToString(),
                WorkAddressUbigeoCode = c.WorkAddressUbigeoCode,
                WorkAddress = c.WorkAddress,
                ContactName = c.ContactName,
                ContactPhone = c.ContactPhone,
                ContactEmail = c.ContactEmail,
                Reference = c.Reference,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                DispatchDate = c.DispatchDate,
                FleteCode = c.FleteCode,
                IgvRate = c.IgvRate,
                ProcessDate = c.ProcessDate,
                GrossAmount = c.GrossAmount,
                IgvAmount = c.IgvAmount,
                NetAmount = c.NetAmount,
                Status = c.Status.ToString(),
                OriginNegocioCode = c.OriginNegocioCode,
                OriginYear = c.OriginYear,
                OriginMonth = c.OriginMonth,
                OriginCode = c.OriginCode,
                ApprovedBy = c.ApprovedBy,
                ApprovedAt = c.ApprovedAt,
                CancelReason = c.CancelReason,
                CanceledBy = c.CanceledBy,
                CanceledAt = c.CanceledAt,
                IsPrinted = c.IsPrinted,
                Observations = c.Observations,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                RowVersion = c.RowVersion
            });

            return await projected.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);
        }
    }
}
