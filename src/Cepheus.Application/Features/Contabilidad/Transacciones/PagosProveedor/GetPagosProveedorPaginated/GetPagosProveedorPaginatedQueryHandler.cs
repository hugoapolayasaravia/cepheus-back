using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GetPagosProveedorPaginated
{
    public class GetPagosProveedorPaginatedQueryHandler
        : IRequestHandler<GetPagosProveedorPaginatedQuery, PagedResult<PagoProveedorListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetPagosProveedorPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<PagoProveedorListItemResponse>> Handle(
            GetPagosProveedorPaginatedQuery request, CancellationToken cancellationToken)
        {
            IQueryable<PagoProveedor> query = _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .AsNoTracking()
                .Include(p => p.Proveedor)
                .Include(p => p.Banco);

            if (request.FechaDesde.HasValue && request.FechaHasta.HasValue)
            {
                var desde = request.FechaDesde.Value.Date;
                var hasta = request.FechaHasta.Value.Date;
                query = query.Where(p => p.FechaEmision >= desde && p.FechaEmision <= hasta);
            }

            if (!string.IsNullOrWhiteSpace(request.Ruc))
            {
                var ruc = request.Ruc.Trim();
                query = query.Where(p => p.Proveedor.DocumentNumber == ruc);
            }

            if (!string.IsNullOrWhiteSpace(request.NumeroCheque))
            {
                var cheque = request.NumeroCheque.Trim();
                query = query.Where(p => p.NumeroCheque == cheque);
            }

            if (!string.IsNullOrWhiteSpace(request.NumeroComprobanteRetencion))
            {
                var numero = request.NumeroComprobanteRetencion.Trim();
                query = query.Where(p => p.NumeroComprobanteRetencion == numero);
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var code = request.Code.Trim();
                query = query.Where(p => p.Code == code);
            }

            if (!string.IsNullOrWhiteSpace(request.BancoCode))
            {
                var banco = request.BancoCode.Trim().ToUpper();
                query = query.Where(p => p.BancoCode == banco);
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            // Legacy: order by Nombre_prv.
            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(p => p.Proveedor.LegalName).ThenBy(p => p.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<PagoProveedorListItemResponse>
            {
                Items = paged.Items.Select(p => new PagoProveedorListItemResponse
                {
                    Code = p.Code,
                    ProveedorRuc = p.Proveedor?.DocumentNumber ?? string.Empty,
                    ProveedorName = p.Proveedor?.LegalName ?? string.Empty,
                    FechaEmision = p.FechaEmision,
                    NumeroCheque = p.NumeroCheque,
                    BancoCode = p.BancoCode,
                    BancoName = p.Banco?.Name ?? string.Empty,
                    NumeroComprobanteRetencion = p.NumeroComprobanteRetencion,
                    MonedaCode = p.MonedaCode,
                    Estado = p.Estado,
                    ComprobanteCajaBancosImpreso = p.ComprobanteCajaBancosImpreso,
                    PagoSol = p.PagoSol,
                    RetencionSol = p.RetencionSol,
                    TotalSol = p.TotalSol
                }).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
