using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GetNotasIngresoParaAsiento
{
    public class GetNotasIngresoParaAsientoQueryHandler
        : IRequestHandler<GetNotasIngresoParaAsientoQuery, PagedResult<NotaIngresoAsientoItemResponse>>
    {
        private const string MonedaSoles = "PEN";

        private readonly IUnitOfWork _uow;

        public GetNotasIngresoParaAsientoQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<NotaIngresoAsientoItemResponse>> Handle(
            GetNotasIngresoParaAsientoQuery request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode!.Trim().ToUpper();
            var desde = request.FechaDesde!.Value.Date;
            var hasta = request.FechaHasta!.Value.Date.AddDays(1);
            var soloPendientes = request.SoloPendientes ?? true;

            var query = _uow.Logistica.Transacciones.NotaIngresos.Query()
                .AsNoTracking()
                .Include(n => n.Proveedor)
                .Include(n => n.ComprobantePago)
                .Where(n => n.PlantaCode == planta
                            && n.Estado == EstadoNotaIngreso.Procesado
                            && n.FechaRecepcion >= desde
                            && n.FechaRecepcion < hasta);

            if (soloPendientes)
            {
                query = query.Where(n => n.AsientoContable == null || n.AsientoContable == "");
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(n => n.PlantaCode).ThenBy(n => n.Code)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<NotaIngresoAsientoItemResponse>
            {
                Items = paged.Items.Select(Map).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }

        private static NotaIngresoAsientoItemResponse Map(Cepheus.Domain.Logistica.Transacciones.NotaIngreso n)
        {
            // Columnas IGV, NETO y T_tot del SP: en soles y con signo negativo en la nota de crédito.
            var signo = n.ComprobantePagoCode == AsientoNotaIngresoRules.NotaCreditoComprobanteCode ? -1m : 1m;
            var factor = signo * (string.Equals(n.MonedaCode, MonedaSoles, StringComparison.OrdinalIgnoreCase) ? 1m : n.TipoCambio);

            return new NotaIngresoAsientoItemResponse
            {
                PlantaCode = n.PlantaCode,
                Code = n.Code,
                ComprobantePagoCode = n.ComprobantePagoCode,
                ComprobantePagoName = n.ComprobantePago?.Name,
                NumeroDocumento = n.NumeroDocumento,
                MonedaCode = n.MonedaCode,
                FechaRecepcion = n.FechaRecepcion,
                FechaEmision = n.FechaEmision,
                FechaDocumento = n.FechaDocumento,
                TipoCambio = n.TipoCambio,
                Igv = n.Igv,
                Total = n.Total,
                Monto = n.Monto,
                NoGravable = n.NoGravable,
                Renta = n.Renta,
                Fonavi = n.Fonavi,
                Servicio = n.Servicio,
                IgvExterior = n.IgvExterior,
                ProveedorCode = n.ProveedorCode,
                ProveedorName = n.Proveedor?.LegalName,
                FormaPagoCode = n.FormaPagoCode,
                AsientoContable = n.AsientoContable,
                NumeroReferencia = n.NumeroReferencia,
                Origen = n.Origen,
                IgvSol = Math.Round(n.Igv * factor, 2, MidpointRounding.AwayFromZero),
                NetoSol = Math.Round(n.Total * factor, 2, MidpointRounding.AwayFromZero),
                TotalSol = Math.Round(n.Monto * factor, 2, MidpointRounding.AwayFromZero)
            };
        }
    }
}
