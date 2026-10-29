using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.GetDocumentosCompraPaginated
{
    public class GetDocumentosCompraPaginatedQueryHandler
        : IRequestHandler<GetDocumentosCompraPaginatedQuery, PagedResult<DocumentoCompraListItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetDocumentosCompraPaginatedQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<DocumentoCompraListItemResponse>> Handle(
            GetDocumentosCompraPaginatedQuery request, CancellationToken cancellationToken)
        {
            IQueryable<DocumentoCompra> query = _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                .AsNoTracking()
                .Include(d => d.Proveedor);

            if (!string.IsNullOrWhiteSpace(request.ComprobantePagoCode))
            {
                var tdo = request.ComprobantePagoCode.Trim().ToUpper();
                query = query.Where(d => d.ComprobantePagoCode == tdo);
            }

            if (!string.IsNullOrWhiteSpace(request.NumeroDocumento))
            {
                var numero = request.NumeroDocumento.Trim();
                query = query.Where(d => d.NumeroDocumento == numero);
            }

            if (!string.IsNullOrWhiteSpace(request.ProveedorCode))
            {
                var prv = request.ProveedorCode.Trim().ToUpper();
                query = query.Where(d => d.ProveedorCode == prv);
            }

            if (!string.IsNullOrWhiteSpace(request.Estado)
                && System.Enum.TryParse<EstadoDocumentoCompra>(request.Estado, true, out var estado))
            {
                query = query.Where(d => d.Estado == estado);
            }

            var sortDesc = request.SortDesc ?? true;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            // El SP no define orden; por defecto: emisión más reciente primero.
            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderByDescending(d => d.FechaEmision).ThenBy(d => d.NumeroDocumento)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            // Venta_dco del legacy: el documento guarda solo la fecha; el valor se consulta en Comunes.TipoCambio
            // con una sola consulta para las fechas de la página.
            var fechas = paged.Items
                .Where(d => d.TipoCambioFecha.HasValue)
                .Select(d => d.TipoCambioFecha!.Value)
                .Distinct()
                .ToList();

            var monedas = paged.Items
                .Where(d => d.TipoCambioFecha.HasValue)
                .Select(d => d.MonedaCode.Trim().ToUpper())
                .Distinct()
                .ToList();

            // El tipo de cambio es por moneda: la clave es (moneda, fecha).
            var tiposCambio = fechas.Count == 0
                ? new Dictionary<(string Moneda, DateOnly Fecha), decimal>()
                : await _uow.Comunes.TiposCambio.Query()
                    .AsNoTracking()
                    .Where(t => fechas.Contains(t.Date) && monedas.Contains(t.MonedaCode))
                    .ToDictionaryAsync(t => (t.MonedaCode, t.Date), t => t.SellRate, cancellationToken);

            return new PagedResult<DocumentoCompraListItemResponse>
            {
                Items = paged.Items
                    .Select(d =>
                    {
                        var item = DocumentoCompraMapper.MapListItem(d);
                        if (string.Equals(d.MonedaCode, DocumentoCompraRules.MonedaSoles, StringComparison.OrdinalIgnoreCase))
                        {
                            item.TipoCambioVenta = 1m;
                        }
                        else if (d.TipoCambioFecha.HasValue && tiposCambio.TryGetValue((d.MonedaCode.Trim().ToUpper(), d.TipoCambioFecha.Value), out var venta))
                        {
                            item.TipoCambioVenta = venta;
                        }
                        return item;
                    })
                    .ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
