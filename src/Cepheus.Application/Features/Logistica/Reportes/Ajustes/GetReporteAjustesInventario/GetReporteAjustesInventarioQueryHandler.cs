using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.Ajustes.GetReporteAjustesInventario;

/// <summary>Solo lectura y sin SQL dinámico: reemplaza Logi_sp_Rpt_Ajuste.</summary>
public sealed class GetReporteAjustesInventarioQueryHandler
    : IRequestHandler<GetReporteAjustesInventarioQuery, ReporteAjustesInventarioResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteAjustesInventarioQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteAjustesInventarioResponse> Handle(GetReporteAjustesInventarioQuery request, CancellationToken ct)
    {
        var planta = ReporteRules.Normalizar(request.PlantaCode);
        var (desde, hastaExcl) = ReporteRules.Rango(request.FechaDesde, request.FechaHasta);

        var q = _uow.Logistica.Transacciones.AjusteInventarioDetalles.Query().AsNoTracking()
            .Where(d => d.PlantaCode == planta
                        && d.Estado == EstadoAjusteInventarioDetalle.Procesado
                        && d.Ajuste.FechaEntrega >= desde
                        && d.Ajuste.FechaEntrega < hastaExcl);

        if (!ReporteRules.EsTodos(request.Tipo))
        {
            var tipo = Enum.Parse<TipoAjusteInventario>(request.Tipo!.Trim(), true);
            q = q.Where(d => d.Tipo == tipo);
        }

        var datos = await q.Select(d => new
        {
            d.PlantaCode,
            d.AjusteCode,
            d.Ajuste.FechaEntrega,
            d.Ajuste.Neto,
            d.Ajuste.CreatedBy,
            d.ArticuloCode,
            ArticuloNombre = d.Articulo.Name,
            d.Articulo.UnidadMedidaCode,
            d.Cantidad, d.Precio, d.Total, d.Tipo
        }).ToListAsync(ct);

        var filas = datos
            .OrderBy(x => x.AjusteCode).ThenBy(x => x.ArticuloCode)
            .Select(x => new ReporteAjusteInventarioFilaResponse(
                x.PlantaCode, x.AjusteCode, x.FechaEntrega, x.Neto, x.CreatedBy, x.ArticuloCode, x.ArticuloNombre,
                x.UnidadMedidaCode, x.Cantidad, x.Precio, x.Total, ReporteRules.Descripcion(x.Tipo)))
            .ToList();

        return new ReporteAjustesInventarioResponse(
            planta, desde, hastaExcl.AddDays(-1), filas.Count, filas.Sum(f => f.Total), filas);
    }
}
