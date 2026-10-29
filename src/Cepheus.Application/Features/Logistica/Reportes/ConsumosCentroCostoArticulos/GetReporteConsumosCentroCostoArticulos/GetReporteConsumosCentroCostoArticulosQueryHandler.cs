using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosCentroCostoArticulos.GetReporteConsumosCentroCostoArticulos;

/// <summary>Solo lectura y sin SQL dinámico: reemplaza Logi_sp_Rpt_Consumos_CC_SCC_Articulos.</summary>
public sealed class GetReporteConsumosCentroCostoArticulosQueryHandler
    : IRequestHandler<GetReporteConsumosCentroCostoArticulosQuery, ReporteConsumosCentroCostoArticulosResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteConsumosCentroCostoArticulosQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteConsumosCentroCostoArticulosResponse> Handle(
        GetReporteConsumosCentroCostoArticulosQuery request, CancellationToken ct)
    {
        var (desde, hastaExcl) = ReporteRules.Rango(request.FechaDesde, request.FechaHasta);
        var tipoFecha = TipoFechaConsumo.Normalizar(request.TipoFecha);
        var planta = ReporteRules.Filtro(request.PlantaCode);
        var cc = ReporteRules.Filtro(request.CentroCostoCode);
        var scc = ReporteRules.Filtro(request.SubCentroCostoCode);
        var articulo = ReporteRules.Filtro(request.ArticuloCode);
        var fam = ReporteRules.Filtro(request.FamiliaCode);
        var sfa = ReporteRules.Filtro(request.SubFamiliaCode);
        var trabajador = ReporteRules.Filtro(request.TrabajadorCode);
        var tipoVale = ReporteRules.Filtro(request.TipoValeCode);
        var plantaAfe = ReporteRules.Filtro(request.PlantaAfectadaCode);
        var tipoOrden = ReporteRules.Filtro(request.TipoOrdenCode);
        var fuente = ReporteRules.Filtro(request.Fuente) ?? "T";

        var filas = new List<ReporteConsumoCentroCostoArticuloFilaResponse>();

        if (fuente is "T" or "N")
        {
            var q = _uow.Logistica.Transacciones.ValeDetalles.Query().AsNoTracking().AsQueryable();

            if (planta is not null) q = q.Where(d => d.PlantaCode == planta);

            q = tipoFecha switch
            {
                TipoFechaConsumo.Proceso => q.Where(d => d.Vale.FechaProcesado >= desde && d.Vale.FechaProcesado < hastaExcl),
                TipoFechaConsumo.Aprobacion => q.Where(d => d.Vale.FechaAprobacion >= desde && d.Vale.FechaAprobacion < hastaExcl),
                _ => q.Where(d => d.Vale.FechaEntrega >= desde && d.Vale.FechaEntrega < hastaExcl)
            };

            if (cc is not null) q = q.Where(d => d.Vale.SubCentroCosto.CentroCostoCode == cc);
            if (scc is not null) q = q.Where(d => d.Vale.SubCentroCostoCode == scc);
            if (articulo is not null) q = q.Where(d => d.ArticuloCode.Contains(articulo));
            if (fam is not null) q = q.Where(d => d.Articulo.SubFamilia.FamiliaCode == fam);
            if (sfa is not null) q = q.Where(d => d.Articulo.SubFamiliaCode == sfa);
            if (trabajador is not null) q = q.Where(d => d.Vale.TrabajadorCode == trabajador);
            if (tipoVale is not null) q = q.Where(d => d.Vale.TipoValeCode == tipoVale);
            if (plantaAfe is not null) q = q.Where(d => d.Vale.PlantaAfectadaCode == plantaAfe);
            if (tipoOrden is not null) q = q.Where(d => d.Vale.OrdenTrabajo != null && d.Vale.OrdenTrabajo.TipoOrdenCode == tipoOrden);
            if (request.EstadoDetalleCode.HasValue)
            {
                var e = (EstadoValeDetalle)request.EstadoDetalleCode.Value;
                q = q.Where(d => d.Estado == e);
            }
            if (request.EstadoCode.HasValue)
            {
                var e = (EstadoVale)request.EstadoCode.Value;
                q = q.Where(d => d.Vale.Estado == e);
            }

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
                d.Vale.Trabajador,
                SfaCode = d.Articulo.SubFamiliaCode,
                SfaNombre = d.Articulo.SubFamilia.Name,
                FamCode = d.Articulo.SubFamilia.FamiliaCode,
                FamNombre = d.Articulo.SubFamilia.Familia.Name,
                d.Vale.OrdenTrabajoCode,
                d.Vale.Estado,
                d.Vale.TipoValeCode
            }).ToListAsync(ct);

            filas.AddRange(datos.Select(x => new ReporteConsumoCentroCostoArticuloFilaResponse(
                "VS", x.ValeCode,
                TipoFechaConsumo.Elegir(tipoFecha, x.FechaEntrega, x.FechaProcesado, x.FechaAprobacion) ?? x.FechaEntrega,
                x.PlantaAfectadaCode, x.PlantaAfectadaNombre, x.CcCode, x.CcNombre, x.SccCode, x.SccNombre,
                x.ArticuloCode, x.ArticuloNombre, x.UnidadMedidaCode, x.Cantidad, x.Precio, x.Total,
                ReporteRules.NombreTrabajador(x.Trabajador),
                x.SfaCode, x.SfaNombre, x.FamCode, x.FamNombre,
                x.OrdenTrabajoCode, ReporteRules.Descripcion(x.Estado), x.TipoValeCode)));
        }

        if (fuente is "T" or "S")
        {
            var q = _uow.Logistica.Transacciones.OrdenServicioSalidaDetalles.Query().AsNoTracking().AsQueryable();

            if (planta is not null) q = q.Where(d => d.PlantaCode == planta);

            q = tipoFecha switch
            {
                TipoFechaConsumo.Proceso => q.Where(d => d.Salida.FechaProcesado >= desde && d.Salida.FechaProcesado < hastaExcl),
                TipoFechaConsumo.Aprobacion => q.Where(d => d.Salida.FechaAprobacion >= desde && d.Salida.FechaAprobacion < hastaExcl),
                _ => q.Where(d => d.Salida.FechaEntrega >= desde && d.Salida.FechaEntrega < hastaExcl)
            };

            if (cc is not null) q = q.Where(d => d.SubCentroCosto.CentroCostoCode == cc);
            if (scc is not null) q = q.Where(d => d.SubCentroCostoCode == scc);
            if (articulo is not null) q = q.Where(d => d.ArticuloCode.Contains(articulo));
            if (fam is not null) q = q.Where(d => d.Articulo.SubFamilia.FamiliaCode == fam);
            if (sfa is not null) q = q.Where(d => d.Articulo.SubFamiliaCode == sfa);
            if (trabajador is not null) q = q.Where(d => d.Salida.TrabajadorCode == trabajador);
            if (tipoVale is not null) q = q.Where(d => d.TipoValeCode == tipoVale);
            if (plantaAfe is not null) q = q.Where(d => d.PlantaAfectadaCode == plantaAfe);
            if (tipoOrden is not null) q = q.Where(d => d.OrdenTrabajo != null && d.OrdenTrabajo.TipoOrdenCode == tipoOrden);
            if (request.EstadoDetalleCode.HasValue)
            {
                var e = (EstadoOrdenServicioSalidaDetalle)request.EstadoDetalleCode.Value;
                q = q.Where(d => d.Estado == e);
            }
            if (request.EstadoCode.HasValue)
            {
                var e = (EstadoOrdenServicioSalida)request.EstadoCode.Value;
                q = q.Where(d => d.Salida.Estado == e);
            }

            var datos = await q.Select(d => new
            {
                ValeCode = d.SalidaCode,
                d.Salida.FechaEntrega, d.Salida.FechaProcesado, d.Salida.FechaAprobacion,
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
                d.Salida.Trabajador,
                SfaCode = d.Articulo.SubFamiliaCode,
                SfaNombre = d.Articulo.SubFamilia.Name,
                FamCode = d.Articulo.SubFamilia.FamiliaCode,
                FamNombre = d.Articulo.SubFamilia.Familia.Name,
                d.OrdenTrabajoCode,
                Estado = d.Salida.Estado,
                d.TipoValeCode
            }).ToListAsync(ct);

            filas.AddRange(datos.Select(x => new ReporteConsumoCentroCostoArticuloFilaResponse(
                "OS", x.ValeCode,
                TipoFechaConsumo.Elegir(tipoFecha, x.FechaEntrega, x.FechaProcesado, x.FechaAprobacion) ?? x.FechaEntrega,
                x.PlantaAfectadaCode, x.PlantaAfectadaNombre, x.CcCode, x.CcNombre, x.SccCode, x.SccNombre,
                x.ArticuloCode, x.ArticuloNombre, x.UnidadMedidaCode, x.Cantidad, x.Precio, x.Total,
                ReporteRules.NombreTrabajador(x.Trabajador),
                x.SfaCode, x.SfaNombre, x.FamCode, x.FamNombre,
                x.OrdenTrabajoCode, ReporteRules.Descripcion(x.Estado), x.TipoValeCode)));
        }

        // Legacy: ORDER BY 1, 2 desc (vale y fecha como texto dd/MM/yyyy); acá la fecha se ordena como fecha.
        var ordenadas = filas.OrderBy(f => f.ValeCode).ThenByDescending(f => f.Fecha).ThenBy(f => f.ArticuloCode).ToList();

        return new ReporteConsumosCentroCostoArticulosResponse(
            desde, hastaExcl.AddDays(-1), ordenadas.Count, ordenadas.Sum(f => f.Total), ordenadas);
    }
}
