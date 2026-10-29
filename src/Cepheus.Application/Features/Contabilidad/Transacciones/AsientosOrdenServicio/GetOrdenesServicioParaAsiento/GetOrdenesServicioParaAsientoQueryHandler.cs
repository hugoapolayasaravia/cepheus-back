using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.GetOrdenesServicioParaAsiento
{
    public class GetOrdenesServicioParaAsientoQueryHandler
        : IRequestHandler<GetOrdenesServicioParaAsientoQuery, PagedResult<OrdenServicioAsientoItemResponse>>
    {
        private const string MonedaSoles = "PEN";

        private readonly IUnitOfWork _uow;

        public GetOrdenesServicioParaAsientoQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<OrdenServicioAsientoItemResponse>> Handle(
            GetOrdenesServicioParaAsientoQuery request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode!.Trim().ToUpper();
            var desde = request.FechaDesde!.Value.Date;
            var hasta = request.FechaHasta!.Value.Date.AddDays(1);
            var soloPendientes = request.SoloPendientes ?? true;

            var query = _uow.Logistica.Transacciones.OrdenesServicio.Query()
                .AsNoTracking()
                .Include(o => o.Proveedor)
                .Include(o => o.ComprobantePago)
                .Where(o => o.PlantaCode == planta
                            && o.Estado == EstadoOrdenServicio.Procesado
                            && o.FechaRecepcion >= desde
                            && o.FechaRecepcion < hasta);

            if (soloPendientes)
            {
                query = query.Where(o => o.AsientoContable == null || o.AsientoContable == "");
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(o => o.PlantaCode).ThenBy(o => o.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<OrdenServicioAsientoItemResponse>
            {
                Items = paged.Items.Select(Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }

        private static OrdenServicioAsientoItemResponse Map(OrdenServicio o)
        {
            // Columnas IGV, NETO y T_tot del SP: en soles y con signo negativo en la nota de crédito.
            var signo = o.ComprobantePagoCode == AsientoNotaIngresoRules.NotaCreditoComprobanteCode ? -1m : 1m;
            var factor = signo * (string.Equals(o.MonedaCode, MonedaSoles, StringComparison.OrdinalIgnoreCase) ? 1m : o.TipoCambio);

            return new OrdenServicioAsientoItemResponse
            {
                PlantaCode = o.PlantaCode,
                Code = o.Code,
                ComprobantePagoCode = o.ComprobantePagoCode,
                ComprobantePagoName = o.ComprobantePago?.Name,
                NumeroDocumento = o.NumeroDocumento,
                MonedaCode = o.MonedaCode,
                FechaRecepcion = o.FechaRecepcion,
                FechaEmision = o.FechaEmision,
                TipoCambio = o.TipoCambio,
                Igv = o.Igv,
                Total = o.Total,
                Monto = o.Monto,
                NoGravable = o.NoGravable,
                Renta = o.Renta,
                Fonavi = o.Fonavi,
                Servicio = o.Servicio,
                IgvExterior = o.IgvExterior,
                ProveedorCode = o.ProveedorCode,
                ProveedorName = o.Proveedor?.LegalName,
                FormaPagoCode = o.FormaPagoCode,
                AsientoContable = o.AsientoContable,
                IgvSol = Math.Round(o.Igv * factor, 2, MidpointRounding.AwayFromZero),
                NetoSol = Math.Round(o.Total * factor, 2, MidpointRounding.AwayFromZero),
                TotalSol = Math.Round(o.Monto * factor, 2, MidpointRounding.AwayFromZero)
            };
        }
    }
}
