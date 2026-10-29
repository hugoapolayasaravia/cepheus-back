using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.GetAprobadoresPorCombinacion;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.ResolverRangoAprobacion;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.Common;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicio.ChangeEstadoOrdenServicio;

/// <summary>
/// Transiciones:
///   Pendiente -> Aprobado | Anulado
///   Aprobado  -> Anulado            (a Procesado se pasa con la acción Procesar)
///   Procesado -> Cerrado | Anulado
///   Cerrado y Anulado son finales.
///
/// Aprobar valida al usuario contra la matriz de aprobación (TipoTransaccion "OS", unidad de negocio,
/// moneda y Monto de la orden), igual que Orden de Compra. Reemplaza a Logi_sp_Verifica_Aprobacion_Transaccion.
///
/// Anular replica Logi_sp_Anula_OrdenServicioI y ue_set_btn_eli_bor del PowerBuilder: anula la orden y sus
/// líneas y su vale de salida. Si ya estaba Procesada exige que no tenga asiento contable y que el período esté abierto, y
/// revierte los materiales de la OT. NO revierte stock: el procesamiento no cambia las cantidades
/// (suma y resta lo mismo) y el legacy tampoco revierte el costo promedio.
/// </summary>
public sealed class ChangeEstadoOrdenServicioCommandHandler
    : IRequestHandler<ChangeEstadoOrdenServicioCommand, OrdenServicioResponse>
{
    private static readonly Dictionary<EstadoOrdenServicio, EstadoOrdenServicio[]> ValidTransitions = new()
    {
        [EstadoOrdenServicio.Pendiente] = new[] { EstadoOrdenServicio.Aprobado, EstadoOrdenServicio.Anulado },
        [EstadoOrdenServicio.Aprobado] = new[] { EstadoOrdenServicio.Anulado },
        [EstadoOrdenServicio.Procesado] = new[] { EstadoOrdenServicio.Cerrado, EstadoOrdenServicio.Anulado },
        [EstadoOrdenServicio.Cerrado] = Array.Empty<EstadoOrdenServicio>(),
        [EstadoOrdenServicio.Anulado] = Array.Empty<EstadoOrdenServicio>()
    };

    private readonly IUnitOfWork _uow;
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUser;

    public ChangeEstadoOrdenServicioCommandHandler(IUnitOfWork uow, ISender sender, ICurrentUserService currentUser)
    {
        _uow = uow;
        _sender = sender;
        _currentUser = currentUser;
    }

    public async Task<OrdenServicioResponse> Handle(ChangeEstadoOrdenServicioCommand request, CancellationToken ct)
    {
        var planta = OrdenServicioRules.Normalize(request.PlantaCode);
        var code = OrdenServicioRules.Normalize(request.Code);

        if (!OrdenServicioRules.TryParseEstado(request.NuevoEstado, out var nuevoEstado))
            throw new ArgumentException($"Estado '{request.NuevoEstado}' no es válido.");

        if (nuevoEstado == EstadoOrdenServicio.Procesado)
            throw new InvalidOperationException(
                "Para procesar la Orden de Servicio use la acción Procesar (actualiza costos de stock y materiales de la OT).");

        var os = await _uow.Logistica.Transacciones.OrdenesServicio.Query()
            .Include(x => x.Detalles)
            .Include(x => x.ValeSalida).ThenInclude(v => v.Detalles)
            .FirstOrDefaultAsync(x => x.PlantaCode == planta && x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Orden de Servicio {planta}/{code} no existe.");

        if (!ValidTransitions[os.Estado].Contains(nuevoEstado))
        {
            var validas = ValidTransitions[os.Estado].Length == 0
                ? "ninguna"
                : string.Join(", ", ValidTransitions[os.Estado]);
            throw new InvalidOperationException(
                $"No se puede pasar de '{os.Estado}' a '{nuevoEstado}'. Transiciones válidas: {validas}.");
        }

        switch (nuevoEstado)
        {
            case EstadoOrdenServicio.Aprobado:
                await ValidarAprobadorAsync(os, ct);
                os.AprobadoPor = _currentUser.FullName;
                os.FechaAprobacion = DateTime.Now;
                break;

            case EstadoOrdenServicio.Anulado:
                await AnularAsync(os, ct);
                break;
        }

        os.Estado = nuevoEstado;

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "La Orden de Servicio fue modificada por otro usuario. Recargue los datos e intente nuevamente.");
        }

        return await OrdenServicioReader.GetAsync(_uow, planta, code, ct);
    }

    private async Task AnularAsync(OrdenServicio os, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(os.AsientoContable))
            throw new InvalidOperationException(
                "La Orden de Servicio ya generó asiento contable. Consulte con contabilidad; no se puede anular.");

        var lineas = os.Detalles.Where(d => d.Estado != EstadoOrdenServicioDetalle.Anulado).ToList();

        if (os.Estado == EstadoOrdenServicio.Procesado)
        {
            await NotaIngresoRules.EnsurePeriodOpenAsync(
                _uow, os.PlantaCode, os.FechaRecepcion,
                "La Orden de Servicio está procesada y su período está cerrado. Imposible su anulación.", ct);

            await OrdenServicioOtMaterialSync.RevertAsync(
                _uow, os, lineas.Where(l => l.Estado == EstadoOrdenServicioDetalle.Procesado), ct);
        }

        foreach (var linea in lineas)
            linea.Estado = EstadoOrdenServicioDetalle.Anulado;

        // Vale de salida: sus líneas procesadas pasan a Devuelto (14) y las demás a Anulado; el vale a Anulado.
        OrdenServicioSalidaSync.MarkAnulado(os);
    }

    private async Task ValidarAprobadorAsync(OrdenServicio os, CancellationToken ct)
    {
        if (_currentUser.UserId is null)
            throw new InvalidOperationException("No se pudo identificar al usuario autenticado.");

        var trabajadorCode = await _uow.Administracion.Users.Query()
            .AsNoTracking()
            .Where(u => u.Id == _currentUser.UserId.Value)
            .Select(u => u.TrabajadorCode)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrWhiteSpace(trabajadorCode))
            throw new InvalidOperationException(
                "Tu usuario no tiene un Trabajador de RRHH vinculado — no puedes aprobar Órdenes de Servicio.");

        var rango = await _sender.Send(
            new ResolverRangoAprobacionQuery(
                OrdenServicioRules.TipoTransaccionOS, os.UnidadNegocioCode, os.MonedaCode, os.Monto),
            ct);

        if (rango.RangoAplicable is null)
            throw new InvalidOperationException(
                $"No existe un rango de aprobación configurado que cubra {os.Monto:N2} " +
                $"para la unidad de negocio {os.UnidadNegocioCode} en {os.MonedaCode} " +
                $"(tipo de transacción {OrdenServicioRules.TipoTransaccionOS}).");

        var aprobadores = await _sender.Send(
            new GetAprobadoresPorCombinacionQuery(
                rango.RangoAplicable.NivelCode, OrdenServicioRules.TipoTransaccionOS,
                rango.UnidadNegocioEfectivaCode, os.MonedaCode),
            ct);

        var autorizado = aprobadores.Any(a =>
            a.TrabajadorCode == trabajadorCode ||
            a.SuplenteTrabajadorCode == trabajadorCode ||
            a.SuperiorTrabajadorCode == trabajadorCode);

        if (!autorizado)
            throw new InvalidOperationException(
                $"No estás autorizado para aprobar esta Orden de Servicio (nivel {rango.RangoAplicable.NivelCode}, " +
                $"monto {os.Monto:N2} {os.MonedaCode}).");
    }
}
