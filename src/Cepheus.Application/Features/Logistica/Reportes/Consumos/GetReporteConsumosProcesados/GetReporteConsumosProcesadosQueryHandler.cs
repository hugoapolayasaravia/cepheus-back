using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.Consumos.GetReporteConsumosProcesados;

/// <summary>Solo lectura y sin SQL dinámico: reemplaza Logi_sp_Rpt_Consumo.</summary>
public sealed class GetReporteConsumosProcesadosQueryHandler
    : IRequestHandler<GetReporteConsumosProcesadosQuery, ReporteConsumosProcesadosResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteConsumosProcesadosQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteConsumosProcesadosResponse> Handle(
        GetReporteConsumosProcesadosQuery request, CancellationToken ct)
    {
        var (desde, hastaExcl) = ReporteRules.Rango(request.FechaDesde, request.FechaHasta);
        var tipoFecha = TipoFechaConsumo.Normalizar(request.TipoFecha);
        var planta = ReporteRules.Filtro(request.PlantaCode);
        var cc = ReporteRules.Filtro(request.CentroCostoCode);
        var scc = ReporteRules.Filtro(request.SubCentroCostoCode);
        var tipoVale = ReporteRules.Filtro(request.TipoValeCode);
        var plantaAfe = ReporteRules.Filtro(request.PlantaAfectadaCode);
        var fam = ReporteRules.Filtro(request.FamiliaCode);
        var sfa = ReporteRules.Filtro(request.SubFamiliaCode);
        var fuente = ReporteRules.Filtro(request.Fuente) ?? "T";

        var filas = new List<ReporteConsumoProcesadoFilaResponse>();

        if (fuente is "T" or "N")
        {
            var q = _uow.Logistica.Transacciones.ValeDetalles.Query().AsNoTracking()
                .Where(d => d.Estado == EstadoValeDetalle.Procesado);

            if (planta is not null) q = q.Where(d => d.PlantaCode == planta);

            q = tipoFecha switch
            {
                TipoFechaConsumo.Proceso => q.Where(d => d.Vale.FechaProcesado >= desde && d.Vale.FechaProcesado < hastaExcl),
                TipoFechaConsumo.Aprobacion => q.Where(d => d.Vale.FechaAprobacion >= desde && d.Vale.FechaAprobacion < hastaExcl),
                _ => q.Where(d => d.Vale.FechaEntrega >= desde && d.Vale.FechaEntrega < hastaExcl)
            };

            if (cc is not null) q = q.Where(d => d.Vale.SubCentroCosto.CentroCostoCode == cc);
            if (scc is not null) q = q.Where(d => d.Vale.SubCentroCostoCode == scc);
            if (tipoVale is not null) q = q.Where(d => d.Vale.TipoValeCode == tipoVale);
            if (plantaAfe is not null) q = q.Where(d => d.Vale.PlantaAfectadaCode == plantaAfe);
            if (fam is not null) q = q.Where(d => d.Articulo.SubFamilia.FamiliaCode == fam);
            if (sfa is not null) q = q.Where(d => d.Articulo.SubFamiliaCode == sfa);

            var datos = await q.Select(d => new
            {
                d.ValeCode,
                d.Vale.FechaEntrega, d.Vale.FechaProcesado, d.Vale.FechaAprobacion,
                d.Vale.PlantaAfectadaCode,
                PlantaAfectadaNombre = d.Vale.PlantaAfectada.Name,
                CcCode = d.Vale.SubCentroCosto.CentroCostoCode,
                CcNombre = d.Vale.SubCentroCosto.CentroCosto != null ? d.Vale.SubCentroCosto.CentroCosto.Name : null,
                SccCode = d.Vale.SubCentroCostoCode,
                SccNombre = d.Vale.SubCentroCosto.Name,
                d.ArticuloCode,
                ArticuloNombre = d.Articulo.Name,
                d.Articulo.UnidadMedidaCode,
                d.Cantidad, d.Precio, d.Total,
                SfaCode = d.Articulo.SubFamiliaCode,
                SfaNombre = d.Articulo.SubFamilia.Name,
                FamCode = d.Articulo.SubFamilia.FamiliaCode,
                FamNombre = d.Articulo.SubFamilia.Familia.Name
            }).ToListAsync(ct);

            filas.AddRange(datos.Select(x => new ReporteConsumoProcesadoFilaResponse(
                "VS", x.ValeCode,
                TipoFechaConsumo.Elegir(tipoFecha, x.FechaEntrega, x.FechaProcesado, x.FechaAprobacion) ?? x.FechaEntrega,
                x.PlantaAfectadaCode, x.PlantaAfectadaNombre, x.CcCode, x.CcNombre, x.SccCode, x.SccNombre,
                x.ArticuloCode, x.ArticuloNombre, x.UnidadMedidaCode, x.Cantidad, x.Precio, x.Total,
                x.SfaCode, x.SfaNombre, x.FamCode, x.FamNombre)));
        }

        if (fuente is "T" or "S")
        {
            var q = _uow.Logistica.Transacciones.OrdenServicioSalidaDetalles.Query().AsNoTracking()
                .Where(d => d.Estado == EstadoOrdenServicioSalidaDetalle.Procesado);

            if (planta is not null) q = q.Where(d => d.PlantaCode == planta);

            q = tipoFecha switch
            {
                TipoFechaConsumo.Proceso => q.Where(d => d.Salida.FechaProcesado >= desde && d.Salida.FechaProcesado < hastaExcl),
                TipoFechaConsumo.Aprobacion => q.Where(d => d.Salida.FechaAprobacion >= desde && d.Salida.FechaAprobacion < hastaExcl),
                _ => q.Where(d => d.Salida.FechaEntrega >= desde && d.Salida.FechaEntrega < hastaExcl)
            };

            if (cc is not null) q = q.Where(d => d.SubCentroCosto.CentroCostoCode == cc);
            if (scc is not null) q = q.Where(d => d.SubCentroCostoCode == scc);
            if (tipoVale is not null) q = q.Where(d => d.TipoValeCode == tipoVale);
            if (plantaAfe is not null) q = q.Where(d => d.PlantaAfectadaCode == plantaAfe);
            if (fam is not null) q = q.Where(d => d.Articulo.SubFamilia.FamiliaCode == fam);
            if (sfa is not null) q = q.Where(d => d.Articulo.SubFamiliaCode == sfa);

            var datos = await q.Select(d => new
            {
                ValeCode = d.SalidaCode,
                d.Salida.FechaEntrega, d.Salida.FechaProcesado, d.Salida.FechaAprobacion,
                d.Salida.TrabajadorCode,
                d.PlantaAfectadaCode,
                PlantaAfectadaNombre = d.PlantaAfectada.Name,
                CcCode = d.SubCentroCosto.CentroCostoCode,
                CcNombre = d.SubCentroCosto.CentroCosto != null ? d.SubCentroCosto.CentroCosto.Name : null,
                SccCode = d.SubCentroCostoCode,
                SccNombre = d.SubCentroCosto.Name,
                d.ArticuloCode,
                ArticuloNombre = d.Articulo.Name,
                d.Articulo.UnidadMedidaCode,
                d.Cantidad, d.Precio, d.Total,
                SfaCode = d.Articulo.SubFamiliaCode,
                SfaNombre = d.Articulo.SubFamilia.Name,
                FamCode = d.Articulo.SubFamilia.FamiliaCode,
                FamNombre = d.Articulo.SubFamilia.Familia.Name
            }).ToListAsync(ct);

            filas.AddRange(datos.Select(x => new ReporteConsumoProcesadoFilaResponse(
                "OS", x.ValeCode,
                TipoFechaConsumo.Elegir(tipoFecha, x.FechaEntrega, x.FechaProcesado, x.FechaAprobacion) ?? x.FechaEntrega,
                x.PlantaAfectadaCode, x.PlantaAfectadaNombre, x.CcCode, x.CcNombre, x.SccCode, x.SccNombre,
                x.ArticuloCode, x.ArticuloNombre, x.UnidadMedidaCode, x.Cantidad, x.Precio,
                // Legacy: CASE R.Codigo_Tra WHEN 'P0001' THEN Total_Art * -1.
                x.TrabajadorCode == ReporteRules.TrabajadorDevolucionOs ? -x.Total : x.Total,
                x.SfaCode, x.SfaNombre, x.FamCode, x.FamNombre)));
        }

        var ordenadas = filas.OrderBy(f => f.ValeCode).ThenBy(f => f.Fuente).ThenBy(f => f.ArticuloCode).ToList();

        return new ReporteConsumosProcesadosResponse(
            desde, hastaExcl.AddDays(-1), ordenadas.Count, ordenadas.Sum(f => f.Total), ordenadas);
    }
}
