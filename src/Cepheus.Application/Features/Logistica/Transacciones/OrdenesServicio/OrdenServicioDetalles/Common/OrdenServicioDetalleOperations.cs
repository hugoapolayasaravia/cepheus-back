using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.OrdenServicioDetalles.Common;

/// <summary>Carga y validaciones comunes a agregar / modificar / eliminar una línea.</summary>
internal static class OrdenServicioDetalleOperations
{
    /// <summary>
    /// PB (ue_ins_lin_det / ue_mod_lin_det / ue_eli_lin_det): las líneas solo se tocan mientras la orden está
    /// Pendiente (aprobada, procesada, anulada o cerrada se bloquean).
    /// </summary>
    public static async Task<OrdenServicio> LoadEditableAsync(
        IUnitOfWork uow, string plantaCode, string code, CancellationToken ct)
    {
        var os = await uow.Logistica.Transacciones.OrdenesServicio.Query()
            .Include(x => x.Detalles)
            .Include(x => x.ValeSalida).ThenInclude(v => v.Detalles)
            .FirstOrDefaultAsync(x => x.PlantaCode == plantaCode && x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Orden de Servicio {plantaCode}/{code} no existe.");

        if (os.Estado != EstadoOrdenServicio.Pendiente)
            throw new InvalidOperationException(
                $"La Orden de Servicio está en estado '{os.Estado}'; solo se modifican líneas en estado Pendiente.");

        return os;
    }

    /// <summary>Refleja las líneas de la orden en el vale de salida y recalcula sus importes.</summary>
    public static async Task SyncValeAsync(IUnitOfWork uow, OrdenServicio os, CancellationToken ct)
    {
        OrdenServicioSalidaSync.SyncLines(uow, os);
        await OrdenServicioSalidaSync.RecalculateAsync(uow, os.ValeSalida, ct);
    }

    public static async Task SaveAsync(IUnitOfWork uow, CancellationToken ct)
    {
        try
        {
            await uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "La Orden de Servicio fue modificada por otro usuario. Recargue los datos e intente nuevamente.");
        }
        catch (DbUpdateException ex)
        {
            var detalleError = ex.InnerException?.Message ?? ex.Message;
            throw new InvalidOperationException($"Error al guardar la línea: {detalleError}", ex);
        }
    }
}
