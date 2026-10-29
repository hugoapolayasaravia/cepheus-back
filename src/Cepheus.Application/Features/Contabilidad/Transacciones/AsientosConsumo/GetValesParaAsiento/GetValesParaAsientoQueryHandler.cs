using Cepheus.Application.Comun.Extensions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.GetValesParaAsiento
{
    public class GetValesParaAsientoQueryHandler
        : IRequestHandler<GetValesParaAsientoQuery, PagedResult<ValeConsumoAsientoItemResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetValesParaAsientoQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagedResult<ValeConsumoAsientoItemResponse>> Handle(
            GetValesParaAsientoQuery request, CancellationToken cancellationToken)
        {
            var planta = request.PlantaCode!.Trim().ToUpper();
            var desde = request.FechaDesde!.Value.Date;
            var hasta = request.FechaHasta!.Value.Date.AddDays(1);
            var soloPendientes = request.SoloPendientes ?? true;

            var query = _uow.Logistica.Transacciones.ValeDetalles.Query()
                .AsNoTracking()
                .Include(d => d.Vale)
                .Where(d => d.PlantaCode == planta
                            && d.Estado == EstadoValeDetalle.Procesado
                            && d.Vale.FechaEntrega >= desde
                            && d.Vale.FechaEntrega < hasta);

            if (soloPendientes)
            {
                query = query.Where(d => d.AsientoContable == null || d.AsientoContable == "");
            }

            var sortDesc = request.SortDesc ?? false;
            var pageNumber = request.PageNumber ?? 1;
            var pageSize = request.PageSize ?? 10;

            var sortedQuery = string.IsNullOrWhiteSpace(request.SortBy)
                ? query.OrderBy(d => d.PlantaCode).ThenBy(d => d.ValeCode).ThenBy(d => d.ItemNumber)
                : query.ApplySort(request.SortBy, sortDesc);

            var paged = await sortedQuery.ToPagedResultAsync(pageNumber, pageSize, cancellationToken);

            return new PagedResult<ValeConsumoAsientoItemResponse>
            {
                Items = paged.Items.Select(d => new ValeConsumoAsientoItemResponse
                {
                    PlantaAfectadaCode = d.Vale.PlantaAfectadaCode,
                    ValeCode = d.ValeCode,
                    SubCentroCostoCode = d.Vale.SubCentroCostoCode,
                    FechaProceso = d.Vale.CreatedAt,
                    FechaEntrega = d.Vale.FechaEntrega,
                    ArticuloCode = d.ArticuloCode,
                    Cantidad = d.Cantidad,
                    Precio = d.Precio,
                    Total = d.Total,
                    AsientoContable = d.AsientoContable,
                    PlantaCode = d.PlantaCode,
                    TipoValeCode = d.Vale.TipoValeCode,
                    ItemNumber = d.ItemNumber
                }).ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }
    }
}
