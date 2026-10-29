using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosAnexos.GetReporteConsumosAnexos;

/// <summary>Solo lectura y sin SQL dinámico: reemplaza Logi_sp_Rpt_Consumo_Gerencial.</summary>
public sealed class GetReporteConsumosAnexosQueryHandler
    : IRequestHandler<GetReporteConsumosAnexosQuery, ReporteConsumosAnexosResponse>
{
    private const string AnexoSinEnlace = "XXXXX";
    private const string CuentaDirecta = "TIENE CUENTA CONTABLE DIRECTA";
    private const string SinEnlace = "NO EXISTE ENLACE";

    private readonly IUnitOfWork _uow;

    public GetReporteConsumosAnexosQueryHandler(IUnitOfWork uow) => _uow = uow;

    private sealed record Cruda(
        string Fuente, string ValeCode, DateTime? FechaProceso, DateTime FechaEntrega, decimal Neto,
        decimal Cantidad, decimal Precio, decimal Total,
        string CcCode, string CcNombre, string SccCode, string SccNombre, string? SccCuenta, string? SccTipo,
        string FamCode, string FamNombre, string SfaCode, string SfaNombre,
        string PlantaAfectadaCode, string? PlantaAfectadaNombre, string ArticuloNombre);

    public async Task<ReporteConsumosAnexosResponse> Handle(GetReporteConsumosAnexosQuery request, CancellationToken ct)
    {
        var planta = ReporteRules.Normalizar(request.PlantaCode);
        var (desde, hastaExcl) = ReporteRules.Rango(request.FechaDesde, request.FechaHasta);
        var cta = ReporteRules.Filtro(request.AccountingAccountCode);
        var tip = ReporteRules.Filtro(request.AccountingAttachmentTypeCode);
        var ane = ReporteRules.Filtro(request.AccountingAttachmentCode);
        var cc = ReporteRules.Filtro(request.CentroCostoCode);
        var scc = ReporteRules.Filtro(request.SubCentroCostoCode);
        var plt = ReporteRules.Filtro(request.PlantaAfectadaCode);

        var crudas = new List<Cruda>();

        // ---- Vales de Salida
        var qv = _uow.Logistica.Transacciones.ValeDetalles.Query().AsNoTracking()
            .Where(d => d.PlantaCode == planta
                        && d.Estado == EstadoValeDetalle.Procesado
                        && d.Vale.FechaEntrega >= desde && d.Vale.FechaEntrega < hastaExcl
                        // Legacy: INNER JOIN a centro de costo y a cuenta contable del subcentro.
                        && d.Vale.SubCentroCosto.CentroCostoCode != null
                        && d.Vale.SubCentroCosto.AccountingAccountCode != null);

        if (cta is not null) qv = qv.Where(d => d.Vale.SubCentroCosto.AccountingAccountCode == cta);
        if (tip is not null) qv = qv.Where(d => d.Vale.SubCentroCosto.AccountingAttachmentTypeCode == tip);
        if (cc is not null) qv = qv.Where(d => d.Vale.SubCentroCosto.CentroCostoCode == cc);
        if (scc is not null) qv = qv.Where(d => d.Vale.SubCentroCostoCode == scc);
        if (plt is not null) qv = qv.Where(d => d.Vale.PlantaAfectadaCode == plt);

        var vales = await qv.Select(d => new Cruda(
            "VS", d.ValeCode, d.Vale.FechaProcesado, d.Vale.FechaEntrega, d.Vale.Neto,
            d.Cantidad, d.Precio, d.Total,
            d.Vale.SubCentroCosto.CentroCostoCode!, d.Vale.SubCentroCosto.CentroCosto!.Name,
            d.Vale.SubCentroCostoCode, d.Vale.SubCentroCosto.Name,
            d.Vale.SubCentroCosto.AccountingAccountCode, d.Vale.SubCentroCosto.AccountingAttachmentTypeCode,
            d.Articulo.SubFamilia.FamiliaCode, d.Articulo.SubFamilia.Familia.Name,
            d.Articulo.SubFamiliaCode, d.Articulo.SubFamilia.Name,
            d.Vale.PlantaAfectadaCode, d.Vale.PlantaAfectada.Name, d.Articulo.Name))
            .ToListAsync(ct);
        crudas.AddRange(vales);

        // ---- Salidas por Orden de Servicio (subcentro y planta afectada van en cada línea)
        var qo = _uow.Logistica.Transacciones.OrdenServicioSalidaDetalles.Query().AsNoTracking()
            .Where(d => d.PlantaCode == planta
                        && d.Estado == EstadoOrdenServicioSalidaDetalle.Procesado
                        && d.Salida.FechaEntrega >= desde && d.Salida.FechaEntrega < hastaExcl
                        && d.SubCentroCosto.CentroCostoCode != null
                        && d.SubCentroCosto.AccountingAccountCode != null);

        if (cta is not null) qo = qo.Where(d => d.SubCentroCosto.AccountingAccountCode == cta);
        if (tip is not null) qo = qo.Where(d => d.SubCentroCosto.AccountingAttachmentTypeCode == tip);
        if (cc is not null) qo = qo.Where(d => d.SubCentroCosto.CentroCostoCode == cc);
        if (scc is not null) qo = qo.Where(d => d.SubCentroCostoCode == scc);
        if (plt is not null) qo = qo.Where(d => d.PlantaAfectadaCode == plt);

        var ordenes = await qo.Select(d => new Cruda(
            "OS", d.SalidaCode, d.Salida.FechaProcesado, d.Salida.FechaEntrega, d.Salida.Neto,
            d.Cantidad, d.Precio, d.Total,
            d.SubCentroCosto.CentroCostoCode!, d.SubCentroCosto.CentroCosto!.Name,
            d.SubCentroCostoCode, d.SubCentroCosto.Name,
            d.SubCentroCosto.AccountingAccountCode, d.SubCentroCosto.AccountingAttachmentTypeCode,
            d.Articulo.SubFamilia.FamiliaCode, d.Articulo.SubFamilia.Familia.Name,
            d.Articulo.SubFamiliaCode, d.Articulo.SubFamilia.Name,
            d.PlantaAfectadaCode, d.PlantaAfectada.Name, d.Articulo.Name))
            .ToListAsync(ct);
        crudas.AddRange(ordenes);

        // ---- Catálogos pequeños: cuentas, tipos de anexo, anexos y enlaces subcentro + subfamilia
        var sccs = crudas.Select(c => c.SccCode).Distinct().ToList();
        var cuentasCodigos = crudas.Select(c => c.SccCuenta!).Distinct().ToList();

        var cuentas = cuentasCodigos.Count == 0
            ? new Dictionary<string, string>()
            : await _uow.Contabilidad.Maestros.CuentasContables.Query().AsNoTracking()
                .Where(c => cuentasCodigos.Contains(c.Code))
                .ToDictionaryAsync(c => c.Code, c => c.Name, ct);

        var tipos = await _uow.Contabilidad.Catalogos.TiposAnexo.Query().AsNoTracking()
            .ToDictionaryAsync(t => t.Code, t => t.Name, ct);

        var anexos = (await _uow.Contabilidad.Maestros.Anexos.Query().AsNoTracking()
                .Select(a => new { a.TipoAnexoCode, a.Code, a.Name })
                .ToListAsync(ct))
            .ToDictionary(a => (a.TipoAnexoCode, a.Code), a => a.Name);

        // Diccionario explícito (no ternario): un ternario entre un diccionario vacío y el resultado de ToDictionary
        // pierde los nombres de los elementos de la tupla.
        var enlaces = new Dictionary<(string, string), (string? Tipo, string? Anexo)>();

        if (sccs.Count > 0)
        {
            var lista = await _uow.Logistica.Maestros.SubFamiliaAnexos.Query().AsNoTracking()
                .Where(e => sccs.Contains(e.SubCentroCostoCode))
                .Select(e => new { e.SubCentroCostoCode, e.SubFamiliaCode, e.AccountingAttachmentTypeCode, e.AccountingAttachmentCode })
                .ToListAsync(ct);

            foreach (var e in lista)
                enlaces[(e.SubCentroCostoCode, e.SubFamiliaCode)] = (e.AccountingAttachmentTypeCode, e.AccountingAttachmentCode);
        }

        var filas = new List<ReporteConsumoAnexoFilaResponse>(crudas.Count);

        foreach (var c in crudas)
        {
            enlaces.TryGetValue((c.SccCode, c.SfaCode), out var enlace);
            var anexoCode = enlace.Anexo;

            // Legacy: el anexo se busca con el tipo del enlace (s_ane.Codigo_Tip), no con el del subcentro.
            string? anexoNombre = null;
            if (!string.IsNullOrWhiteSpace(enlace.Tipo) && !string.IsNullOrWhiteSpace(anexoCode))
                anexos.TryGetValue((enlace.Tipo!, anexoCode!), out anexoNombre);

            var dAnexo = anexoNombre
                ?? (string.IsNullOrWhiteSpace(c.SccTipo) ? CuentaDirecta : SinEnlace);

            var codigoAnexo = string.IsNullOrWhiteSpace(anexoCode) ? AnexoSinEnlace : anexoCode!;

            // Filtro por anexo (S_ane.Codigo_Ane del legacy): solo filas con ese anexo en el enlace.
            if (ane is not null && !string.Equals(anexoCode, ane, StringComparison.OrdinalIgnoreCase))
                continue;

            cuentas.TryGetValue(c.SccCuenta!, out var cuentaNombre);
            string? tipoNombre = null;
            if (!string.IsNullOrWhiteSpace(c.SccTipo)) tipos.TryGetValue(c.SccTipo!, out tipoNombre);

            filas.Add(new ReporteConsumoAnexoFilaResponse(
                c.Fuente, c.ValeCode, c.FechaProceso, c.FechaEntrega, c.Neto, c.Cantidad, c.Precio, c.Total,
                c.CcCode, c.CcNombre, c.SccCode, c.SccNombre, c.FamCode, c.FamNombre, c.SfaCode, c.SfaNombre,
                c.SccCuenta!, cuentaNombre ?? string.Empty, c.SccTipo, tipoNombre,
                codigoAnexo, dAnexo, c.PlantaAfectadaCode, c.PlantaAfectadaNombre, c.ArticuloNombre));
        }

        var ordenadas = filas.OrderBy(f => f.ValeCode).ThenBy(f => f.Fuente).ThenBy(f => f.SubFamiliaCode).ToList();

        return new ReporteConsumosAnexosResponse(
            planta, desde, hastaExcl.AddDays(-1), ordenadas.Count, ordenadas.Sum(f => f.Total), ordenadas);
    }
}
