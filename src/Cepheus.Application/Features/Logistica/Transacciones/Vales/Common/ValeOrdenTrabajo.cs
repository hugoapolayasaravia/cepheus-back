using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Transacciones;
using Cepheus.Domain.Mantenimiento.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;

/// <summary>
/// Vínculo del vale con los materiales de la Orden de Trabajo (OTRMaterial = TOTRMateriales del legacy).
/// Estados del material: 01 pendiente (planificado), 15 asignado a un vale, 13 consumido.
/// </summary>
public static class ValeOrdenTrabajo
{
    public static async Task<OTRMaterial?> FindLinkedAsync(
        IUnitOfWork uow,
        Vale vale,
        ValeDetalle detalle,
        CancellationToken ct)
    {
        if (vale.OrdenTrabajoCode is null || detalle.MaterialOtFechaProceso is null)
        {
            return null;
        }

        return await uow.Mantenimiento.Transacciones.OTRMateriales
            .Query()
            .FirstOrDefaultAsync(
                m => m.PlantaCode == vale.PlantaCode &&
                     m.OrdenTrabajoCode == vale.OrdenTrabajoCode &&
                     m.FechaProceso == detalle.MaterialOtFechaProceso &&
                     m.ArticuloCode == detalle.ArticuloCode,
                ct);
    }

    /// <summary>Línea que nació de un material de la OT: 15 -> 01 (se libera para otro vale).</summary>
    public static async Task ReleaseAsync(
        IUnitOfWork uow,
        Vale vale,
        ValeDetalle detalle,
        CancellationToken ct)
    {
        var material = await FindLinkedAsync(uow, vale, detalle, ct);

        if (material is not null &&
            material.EstadoCode == ValeRules.OtMaterialAsignado)
        {
            material.EstadoCode = ValeRules.OtMaterialPendiente;
        }
    }

    /// <summary>
    /// Procesar (Logi_sp_AgregaTOTRMateriales):
    /// - línea vinculada a un material de la OT: el material pasa a consumido (13);
    /// - línea manual de un vale con OT: se registra el consumo con llave (Planta, OT, FechaEntrega, Artículo):
    ///   si no existe se inserta como consumido; si existe pendiente / asignado pasa a consumido;
    ///   si existe en otro estado se acumulan cantidad y costo.
    /// </summary>
    public static async Task ConsumeAsync(
        IUnitOfWork uow,
        Vale vale,
        ValeDetalle detalle,
        CancellationToken ct)
    {
        if (vale.OrdenTrabajoCode is null)
        {
            return;
        }

        var linked = await FindLinkedAsync(uow, vale, detalle, ct);

        if (linked is not null)
        {
            linked.EstadoCode = ValeRules.OtMaterialConsumido;
            return;
        }

        var existing = await uow.Mantenimiento.Transacciones.OTRMateriales
            .Query()
            .FirstOrDefaultAsync(
                m => m.PlantaCode == vale.PlantaCode &&
                     m.OrdenTrabajoCode == vale.OrdenTrabajoCode &&
                     m.FechaProceso == vale.FechaEntrega &&
                     m.ArticuloCode == detalle.ArticuloCode,
                ct);

        if (existing is null)
        {
            await uow.Mantenimiento.Transacciones.OTRMateriales.AddAsync(
                new OTRMaterial
                {
                    PlantaCode = vale.PlantaCode,
                    OrdenTrabajoCode = vale.OrdenTrabajoCode,
                    FechaProceso = vale.FechaEntrega,
                    ArticuloCode = detalle.ArticuloCode,
                    Cantidad = detalle.Cantidad,
                    CostoUnitario = detalle.Precio,
                    CostoTotal = detalle.Total,
                    EstadoCode = ValeRules.OtMaterialConsumido
                },
                ct);

            return;
        }

        if (existing.EstadoCode == ValeRules.OtMaterialAsignado ||
            existing.EstadoCode == ValeRules.OtMaterialPendiente)
        {
            existing.EstadoCode = ValeRules.OtMaterialConsumido;
            return;
        }

        existing.Cantidad += detalle.Cantidad;
        existing.CostoUnitario = detalle.Precio;
        existing.CostoTotal += detalle.Total;
        existing.EstadoCode = ValeRules.OtMaterialConsumido;
    }

    /// <summary>
    /// Devolver: línea vinculada -> el material vuelve a pendiente (01); línea manual -> se borra el
    /// consumo registrado con llave (Planta, OT, FechaEntrega, Artículo), como el legacy.
    /// </summary>
    public static async Task ReturnAsync(
        IUnitOfWork uow,
        Vale vale,
        ValeDetalle detalle,
        CancellationToken ct)
    {
        if (vale.OrdenTrabajoCode is null)
        {
            return;
        }

        var linked = await FindLinkedAsync(uow, vale, detalle, ct);

        if (linked is not null)
        {
            linked.EstadoCode = ValeRules.OtMaterialPendiente;
            return;
        }

        var consumo = await uow.Mantenimiento.Transacciones.OTRMateriales
            .Query()
            .FirstOrDefaultAsync(
                m => m.PlantaCode == vale.PlantaCode &&
                     m.OrdenTrabajoCode == vale.OrdenTrabajoCode &&
                     m.FechaProceso == vale.FechaEntrega &&
                     m.ArticuloCode == detalle.ArticuloCode,
                ct);

        if (consumo is not null)
        {
            uow.Mantenimiento.Transacciones.OTRMateriales.Remove(consumo);
        }
    }
}
