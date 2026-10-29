using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioEquipos.GetReporteCalendarioEquipos;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Cepheus.Domain.Mantenimiento.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesPreventivas.GenerateOrdenesPreventivas;

/// <summary>
/// Reemplaza el botón "Genera OT" del legacy. Diferencias deliberadas:
///  - El servidor decide qué OT tocan (mismo cálculo del Calendario de Equipos), no el cliente.
///  - Todo (OT, materiales, vale, totales) se confirma con UN solo SaveChanges: no quedan OT sin vale ni vales
///    a medias si algo falla (el legacy hacía commit paso a paso).
///  - Responsable y especialidad salen del plan (el legacy fijaba '00653' y 'V').
///  - Los materiales de la OT quedan "asignados a vale" (15), como en CreateVale, porque el vale los toma.
///  - El vale nace Pendiente y NO valida fecha futura ni stock disponible (igual que el legacy): esas
///    validaciones se aplican al aprobarlo y procesarlo.
/// </summary>
public sealed class GenerateOrdenesPreventivasCommandHandler
    : IRequestHandler<GenerateOrdenesPreventivasCommand, GenerateOrdenesPreventivasResponse>
{
    private const int MaxConcurrencyRetries = 3;

    private readonly IUnitOfWork _uow;

    public GenerateOrdenesPreventivasCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    private sealed record Objetivo(PlanCalendario Plan, DateTime Fecha);

    public async Task<GenerateOrdenesPreventivasResponse> Handle(
        GenerateOrdenesPreventivasCommand request, CancellationToken ct)
    {
        var planta = request.PlantaCode.Trim().ToUpperInvariant();
        var desde = request.FechaDesde.Date;
        var hasta = request.FechaHasta.Date;

        var seleccion = (request.PlanCodes ?? Array.Empty<string>())
            .Select(c => c.Trim().ToUpperInvariant())
            .ToHashSet();

        var calculo = await CalendarioEquiposCalculo.CalcularAsync(_uow, planta, request.EquipoCode, desde, hasta, ct);

        // Legacy: genera cuando dato = 0 o primer día vencido (.99), y nunca donde ya hay una OT del plan.
        var objetivos = calculo.Gamas
            .Where(g => seleccion.Count == 0 || seleccion.Contains(g.Plan.PlanCode))
            .SelectMany(g => g.Celdas
                .Where(c => !c.HayOrden && CalendarioEquiposCalculator.TocaGenerarOrden(c.Dato))
                .Select(c => new Objetivo(g.Plan, c.Fecha)))
            .ToList();

        if (objetivos.Count == 0)
        {
            return new GenerateOrdenesPreventivasResponse(0, 0, Array.Empty<OrdenPreventivaGeneradaResponse>());
        }

        var datos = await CargarDatosAsync(planta, objetivos, ct);

        for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
        {
            var generadas = await ConstruirAsync(planta, objetivos, datos, ct);

            try
            {
                await _uow.SaveChangesAsync(ct);
                return new GenerateOrdenesPreventivasResponse(
                    generadas.Count,
                    generadas.Count(g => g.ValeCode is not null),
                    generadas);
            }
            catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
            {
                // Colisión de correlativo con otro usuario: se reintenta con códigos nuevos.
                _uow.ClearTracking();
            }
        }

        throw new InvalidOperationException(
            $"No se pudieron generar los correlativos de OT/vale de la planta {planta} por alta concurrencia. Intente nuevamente.");
    }

    // ------------------------------------------------------------------ datos de apoyo

    private sealed record DatosGeneracion(
        Dictionary<string, PlanDatos> Planes,
        ILookup<string, MaterialPlan> Materiales,
        Dictionary<string, string> PlantaDeSubCentro,
        Dictionary<string, decimal> CostoPromedio);

    private sealed record PlanDatos(
        string Code,
        string ResponsableCode,
        string EspecialidadCode,
        string SubCentroCostoCode,
        string? SubCentroEjecutorCode);

    private sealed record MaterialPlan(string PlanCode, string ArticuloCode, decimal Cantidad, decimal CostoUnitario);

    private async Task<DatosGeneracion> CargarDatosAsync(string planta, List<Objetivo> objetivos, CancellationToken ct)
    {
        var codigosPlan = objetivos.Select(o => o.Plan.PlanCode).Distinct().ToList();

        var planes = (await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivo.Query().AsNoTracking()
                .Where(p => p.PlantaCode == planta && codigosPlan.Contains(p.Code))
                .Select(p => new PlanDatos(
                    p.Code, p.ResponsableCode, p.EspecialidadCode, p.SubCentroCostoCode, p.SubCentroEjecutorCode))
                .ToListAsync(ct))
            .ToDictionary(p => p.Code);

        var materiales = (await _uow.Mantenimiento.Maestros.PlanesMantenimientoPreventivoMateriales.Query().AsNoTracking()
                .Where(m => m.PlantaCode == planta && codigosPlan.Contains(m.PlanCode))
                .Select(m => new MaterialPlan(m.PlanCode, m.ArticuloCode, m.Cantidad, m.CostoUnitario))
                .ToListAsync(ct))
            .ToLookup(m => m.PlanCode);

        var subCentros = planes.Values.Select(p => p.SubCentroCostoCode).Distinct().ToList();
        var plantaDeSubCentro = await _uow.Logistica.Maestros.SubCentrosCosto.Query().AsNoTracking()
            .Where(s => subCentros.Contains(s.Code))
            .Select(s => new { s.Code, s.PlantaCode })
            .ToDictionaryAsync(s => s.Code, s => s.PlantaCode, ct);

        var articulos = materiales.SelectMany(g => g).Select(m => m.ArticuloCode).Distinct().ToList();
        var stock = await _uow.Logistica.Maestros.StockArticulos.Query().AsNoTracking()
            .Where(s => s.PlantaCode == planta && articulos.Contains(s.ArticuloCode))
            .Select(s => new { s.ArticuloCode, s.AverageCost })
            .ToListAsync(ct);
        var costoPromedio = stock.ToDictionary(s => s.ArticuloCode, s => Convert.ToDecimal(s.AverageCost));

        await ValidarCatalogosAsync(materiales.Any(), ct);

        return new DatosGeneracion(planes, materiales, plantaDeSubCentro, costoPromedio);
    }

    /// <summary>Los códigos fijos del legacy deben existir; si no, se avisa con un mensaje claro (no un error de FK).</summary>
    private async Task ValidarCatalogosAsync(bool conVale, CancellationToken ct)
    {
        if (!await _uow.Mantenimiento.Catalogos.Oportunidades.Query().AnyAsync(x => x.Code == OrdenPreventivaRules.OportunidadCode, ct))
            throw new InvalidOperationException($"No existe la oportunidad {OrdenPreventivaRules.OportunidadCode} requerida para generar OT preventivas.");

        if (!await _uow.Mantenimiento.Catalogos.Prioridades.Query().AnyAsync(x => x.Code == OrdenPreventivaRules.PrioridadCode, ct))
            throw new InvalidOperationException($"No existe la prioridad {OrdenPreventivaRules.PrioridadCode} requerida para generar OT preventivas.");

        if (!await _uow.Mantenimiento.Catalogos.Inspecciones.Query().AnyAsync(x => x.Code == OrdenPreventivaRules.InspeccionCode, ct))
            throw new InvalidOperationException($"No existe la inspección {OrdenPreventivaRules.InspeccionCode} requerida para generar OT preventivas.");

        if (!await _uow.Mantenimiento.Catalogos.TiposOrden.Query().AnyAsync(x => x.Code == OrdenPreventivaRules.TipoOrdenCode, ct))
            throw new InvalidOperationException($"No existe el tipo de orden {OrdenPreventivaRules.TipoOrdenCode} requerido para generar OT preventivas.");

        if (!conVale) return;

        if (!await _uow.Logistica.Catalogos.TiposVale.Query().AnyAsync(x => x.Code == OrdenPreventivaRules.TipoValeCode && x.IsActive, ct))
            throw new InvalidOperationException($"El tipo de vale {OrdenPreventivaRules.TipoValeCode} no existe o está inactivo.");

        if (!await _uow.Logistica.Catalogos.UnidadesNegocio.Query().AnyAsync(x => x.Code == OrdenPreventivaRules.UnidadNegocioCode && x.IsActive, ct))
            throw new InvalidOperationException($"La unidad de negocio {OrdenPreventivaRules.UnidadNegocioCode} no existe o está inactiva.");
    }

    // ------------------------------------------------------------------ construcción

    private async Task<List<OrdenPreventivaGeneradaResponse>> ConstruirAsync(
        string planta, List<Objetivo> objetivos, DatosGeneracion datos, CancellationToken ct)
    {
        var siguienteOt = await SiguienteNumeroOtAsync(planta, ct);
        var siguienteVale = datos.Materiales.Any() ? await SiguienteNumeroValeAsync(planta, ct) : 0;

        var generadas = new List<OrdenPreventivaGeneradaResponse>();

        foreach (var objetivo in objetivos)
        {
            var plan = datos.Planes[objetivo.Plan.PlanCode];
            var otCode = siguienteOt.ToString().PadLeft(OrdenPreventivaRules.OtCodeLength, '0');
            siguienteOt++;

            var descripcion = objetivo.Plan.Gama.Length > OrdenPreventivaRules.OtDescriptionMaxLength
                ? objetivo.Plan.Gama[..OrdenPreventivaRules.OtDescriptionMaxLength]
                : objetivo.Plan.Gama;

            var orden = new OrdenTrabajo
            {
                PlantaCode = planta,
                Code = otCode,
                Description = descripcion,
                FechaProceso = objetivo.Fecha,
                ResponsableCode = plan.ResponsableCode,
                EspecialidadCode = plan.EspecialidadCode,
                OportunidadCode = OrdenPreventivaRules.OportunidadCode,
                EquipoCode = objetivo.Plan.EquipoCode,
                PrioridadCode = OrdenPreventivaRules.PrioridadCode,
                InspeccionCode = OrdenPreventivaRules.InspeccionCode,
                TipoOrdenCode = OrdenPreventivaRules.TipoOrdenCode,
                ActividadCode = objetivo.Plan.ActividadCode,
                SubCentroCostoCode = plan.SubCentroCostoCode,
                SubCentroEjecutorCode = plan.SubCentroEjecutorCode,
                PlanMantenimientoPreventivoCode = plan.Code,
                DowntimeHours = 0,
                Estado = OrdenPreventivaRules.EstadoInicial,
                Turno = OrdenPreventivaRules.TurnoPorDefecto
            };

            await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.AddAsync(orden, ct);

            var materialesPlan = datos.Materiales[plan.Code].ToList();
            string? valeCode = null;

            if (materialesPlan.Count > 0)
            {
                valeCode = ValeRules.CodePrefix + siguienteVale.ToString().PadLeft(ValeRules.CorrelativeLength, '0');
                siguienteVale++;

                var plantaAfectada = datos.PlantaDeSubCentro.TryGetValue(plan.SubCentroCostoCode, out var pa) ? pa : planta;

                var vale = new Vale
                {
                    PlantaCode = planta,
                    Code = valeCode,
                    TipoValeCode = OrdenPreventivaRules.TipoValeCode,
                    FechaEntrega = objetivo.Fecha,
                    SubCentroCostoCode = plan.SubCentroCostoCode,
                    SubCentroEjecutorCode = plan.SubCentroEjecutorCode,
                    TrabajadorCode = plan.ResponsableCode,
                    OrdenTrabajoCode = otCode,
                    UnidadNegocioCode = OrdenPreventivaRules.UnidadNegocioCode,
                    PlantaAfectadaCode = plantaAfectada,
                    Estado = EstadoVale.Pendiente
                };

                foreach (var material in materialesPlan)
                {
                    var precio = material.CostoUnitario > 0
                        ? material.CostoUnitario
                        : datos.CostoPromedio.GetValueOrDefault(material.ArticuloCode);
                    var total = ValeRules.LineTotal(material.Cantidad, precio);

                    await _uow.Mantenimiento.Transacciones.OTRMateriales.AddAsync(new OTRMaterial
                    {
                        PlantaCode = planta,
                        OrdenTrabajoCode = otCode,
                        FechaProceso = objetivo.Fecha,
                        ArticuloCode = material.ArticuloCode,
                        Cantidad = material.Cantidad,
                        CostoUnitario = precio,
                        CostoTotal = total,
                        // El vale toma el material: pasa directo a "asignado a vale" (como CreateVale).
                        EstadoCode = ValeRules.OtMaterialAsignado
                    }, ct);

                    vale.Detalles.Add(new ValeDetalle
                    {
                        PlantaCode = planta,
                        ValeCode = valeCode,
                        ArticuloCode = material.ArticuloCode,
                        Cantidad = material.Cantidad,
                        Precio = precio,
                        Total = total,
                        Estado = EstadoValeDetalle.Pendiente,
                        MaterialOtFechaProceso = objetivo.Fecha
                    });
                }

                ValeLineas.Renumerar(vale);
                await ValeRules.RecalculateTotalsAsync(_uow, vale, ct);

                await _uow.Logistica.Transacciones.Vales.AddAsync(vale, ct);
            }

            generadas.Add(new OrdenPreventivaGeneradaResponse(
                planta,
                plan.Code,
                objetivo.Plan.EquipoCode,
                objetivo.Plan.ActividadCode,
                objetivo.Plan.Gama,
                objetivo.Fecha,
                otCode,
                valeCode,
                materialesPlan.Count));
        }

        return generadas;
    }

    private async Task<int> SiguienteNumeroOtAsync(string planta, CancellationToken ct)
    {
        var ultimo = await _uow.Mantenimiento.Transacciones.OrdenesTrabajo.Query().AsNoTracking()
            .Where(o => o.PlantaCode == planta)
            .OrderByDescending(o => o.Code)
            .Select(o => o.Code)
            .FirstOrDefaultAsync(ct);

        return ultimo is not null && int.TryParse(ultimo, out var n) ? n + 1 : 1;
    }

    private async Task<int> SiguienteNumeroValeAsync(string planta, CancellationToken ct)
    {
        var siguiente = await ValeRules.NextCodeAsync(_uow, planta, ct);
        return int.Parse(siguiente[ValeRules.CodePrefix.Length..]);
    }
}
