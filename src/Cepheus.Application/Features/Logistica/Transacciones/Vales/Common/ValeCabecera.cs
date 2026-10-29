using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Transacciones;
using Cepheus.Domain.Mantenimiento.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;

/// <summary>Cabecera ya validada (catálogos, OT, fechas), lista para asignar al vale.</summary>
public sealed record ValeCabeceraResuelta(
    string TipoValeCode,
    DateTime FechaEntrega,
    string SubCentroCostoCode,
    string? SubCentroEjecutorCode,
    string TrabajadorCode,
    string UnidadNegocioCode,
    string PlantaAfectadaCode);

/// <summary>
/// Validaciones de cabecera del evento de grabado del PB (ue_set_btn_grb_data, modo nuevo / modificación),
/// compartidas por Create y Update.
/// </summary>
public static class ValeCabecera
{
    public static async Task<ValeCabeceraResuelta> ResolverAsync(
        IUnitOfWork uow,
        string plantaCode,
        string tipoValeCode,
        DateTime fechaEntrega,
        string? subCentroCostoCode,
        string? subCentroEjecutorCode,
        string? trabajadorCode,
        string unidadNegocioCode,
        string? plantaAfectadaCode,
        string? ordenTrabajoCode,
        OrdenTrabajo? ordenTrabajo,
        CancellationToken ct)
    {
        var planta = await uow.Comunes.Plantas
            .Query()
            .AsNoTracking()
            .AnyAsync(p => p.Code == plantaCode, ct);

        if (!planta)
        {
            throw new InvalidOperationException($"La planta {plantaCode} no existe.");
        }

        var tipoVale = await uow.Logistica.Catalogos.TiposVale
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Code == tipoValeCode, ct);

        if (tipoVale is null || !tipoVale.IsActive)
        {
            throw new InvalidOperationException(
                $"El tipo de vale {tipoValeCode} no existe o está inactivo.");
        }

        if (tipoValeCode == ValeRules.TipoValeConOt && ordenTrabajoCode is null)
        {
            throw new InvalidOperationException(
                "Debe ingresar la Orden de Trabajo para este tipo de vale.");
        }

        if (tipoValeCode == ValeRules.TipoValeInterno && ordenTrabajoCode is not null)
        {
            throw new InvalidOperationException(
                "Un vale de tipo interno no puede llevar Orden de Trabajo.");
        }

        // Con OT: responsable, subcentro de costo y ejecutor salen de la OT (como el PB).
        var trabajador = ordenTrabajo?.ResponsableCode
            ?? ValeRules.NormalizeOrNull(trabajadorCode)
            ?? throw new InvalidOperationException("Debe indicar el trabajador responsable.");

        var subCentro = ordenTrabajo?.SubCentroCostoCode
            ?? ValeRules.NormalizeOrNull(subCentroCostoCode)
            ?? throw new InvalidOperationException("Debe indicar el sub centro de costo beneficiado.");

        var ejecutor = ordenTrabajo?.SubCentroEjecutorCode
            ?? ValeRules.NormalizeOrNull(subCentroEjecutorCode);

        var unidad = ValeRules.Normalize(unidadNegocioCode);

        if (!await uow.Rrhh.Maestros.Trabajadores
                .Query()
                .AsNoTracking()
                .AnyAsync(t => t.Code == trabajador, ct))
        {
            throw new InvalidOperationException($"El trabajador {trabajador} no existe.");
        }

        var subCentroEntidad = await uow.Logistica.Maestros.SubCentrosCosto
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Code == subCentro, ct);

        if (subCentroEntidad is null)
        {
            throw new InvalidOperationException($"El sub centro de costo {subCentro} no existe.");
        }

        if (!subCentroEntidad.IsActive)
        {
            throw new InvalidOperationException(
                $"El sub centro de costo {subCentro} se encuentra inactivo.");
        }

        if (ejecutor is not null &&
            !await uow.Mantenimiento.Maestros.SubCentrosEjecutores
                .Query()
                .AsNoTracking()
                .AnyAsync(e => e.Code == ejecutor && e.IsActive, ct))
        {
            throw new InvalidOperationException(
                $"El sub centro ejecutor {ejecutor} no existe o está inactivo.");
        }

        if (!await uow.Logistica.Catalogos.UnidadesNegocio
                .Query()
                .AsNoTracking()
                .AnyAsync(u => u.Code == unidad && u.IsActive, ct))
        {
            throw new InvalidOperationException(
                $"La unidad de negocio {unidad} no existe o está inactiva.");
        }

        // Planta beneficiada: por defecto la del subcentro de costo.
        var afectada = ValeRules.NormalizeOrNull(plantaAfectadaCode) ?? subCentroEntidad.PlantaCode;

        if (!await uow.Comunes.Plantas
                .Query()
                .AsNoTracking()
                .AnyAsync(p => p.Code == afectada, ct))
        {
            throw new InvalidOperationException($"La planta afectada {afectada} no existe.");
        }

        // Fecha de entrega: no mayor a la fecha del sistema y posterior al último cierre.
        var entrega = fechaEntrega.Date;

        if (entrega > ValeRules.GetProcessDate())
        {
            throw new InvalidOperationException(
                "La Fecha de Entrega no puede ser mayor a la fecha del sistema.");
        }

        await ValeRules.EnsurePeriodOpenAsync(
            uow,
            plantaCode,
            entrega,
            "La Fecha de Entrega no puede ser menor o igual al cierre del período.",
            ct);

        return new ValeCabeceraResuelta(
            tipoValeCode,
            entrega,
            subCentro,
            ejecutor,
            trabajador,
            unidad,
            afectada);
    }

    /// <summary>La OT del vale debe existir en la planta y estar en ejecución (EJECUCION del legacy).</summary>
    public static async Task<OrdenTrabajo> LoadOrdenTrabajoEnEjecucionAsync(
        IUnitOfWork uow,
        string plantaCode,
        string ordenTrabajoCode,
        CancellationToken ct)
    {
        var ot = await uow.Mantenimiento.Transacciones.OrdenesTrabajo
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                o => o.PlantaCode == plantaCode && o.Code == ordenTrabajoCode,
                ct);

        if (ot is null)
        {
            throw new KeyNotFoundException(
                $"La Orden de Trabajo {ordenTrabajoCode} no existe en la planta {plantaCode}.");
        }

        if (ot.Estado != Cepheus.Domain.Mantenimiento.Enum.EstadoOrdenTrabajo.EnProceso)
        {
            throw new InvalidOperationException(
                "La Orden de Trabajo debe estar en estado de Ejecución.");
        }

        return ot;
    }
}
