using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.Common;

/// <summary>Fila común de la bandeja de aprobación (un documento pendiente de cualquier tipo).</summary>
public sealed record PendienteRow(
    string TipoTransaccion,
    string PlantaCode,
    string Code,
    string UnidadNegocioCode,
    string MonedaCode,
    decimal Monto,
    DateTime Fecha,
    string? Contraparte,
    string? CreatedBy);

/// <summary>
/// Lectura de los documentos pendientes por tipo y filtro "míos / todos".
/// Reemplaza a Parametros_Listado + los SP Listado_* + Logi_sp_Listado_Transaccion_Pendientes del legacy:
/// los pendientes se leen de cada transacción (estado Pendiente, total &gt; 0) y la visibilidad se calcula con
/// <see cref="IAprobacionService"/>.
/// </summary>
public static class BandejaAprobacion
{
    private const string TipoValeCombustible = "006";   // VC
    private const string TipoValeTransferencia = "005"; // TRA: no se aprueba

    public static async Task<List<PendienteRow>> LeerPendientesAsync(
        IUnitOfWork uow,
        IAprobacionService aprobacion,
        string tipo,
        bool todos,
        CancellationToken ct)
    {
        var filas = await LeerTodosAsync(uow, tipo, ct);

        if (todos)
        {
            return filas;
        }

        var trabajadorCode = await aprobacion.ObtenerTrabajadorActualAsync(ct);

        if (string.IsNullOrWhiteSpace(trabajadorCode))
        {
            throw new InvalidOperationException(
                "Tu usuario no tiene un Trabajador de RRHH vinculado — no tiene bandeja de aprobación propia.");
        }

        var propios = new List<PendienteRow>();

        foreach (var fila in filas)
        {
            var resultado = await aprobacion.ObtenerHabilitadosAsync(
                new DocumentoAprobable(
                    fila.TipoTransaccion, fila.PlantaCode, fila.Code,
                    fila.UnidadNegocioCode, fila.MonedaCode, fila.Monto),
                ct);

            if (resultado.Habilitados.Any(h => h.TrabajadorCode == trabajadorCode))
            {
                propios.Add(fila);
            }
        }

        return propios;
    }

    public static async Task<List<PendienteRow>> LeerTodosAsync(
        IUnitOfWork uow,
        string tipo,
        CancellationToken ct)
    {
        var T = uow.Logistica.Transacciones;

        switch (tipo)
        {
            case AprobacionTipos.Pedido:
                return (await T.Pedidos.Query().AsNoTracking()
                        .Where(x => x.EstadoPedido == EstadoPedido.Pendiente && x.TotalPedido > 0)
                        .Select(x => new { x.PlantaCode, x.Code, x.UnidadNegocioCode, x.TotalPedido, x.CreatedAt, x.TrabajadorCode, x.CreatedBy })
                        .ToListAsync(ct))
                    .Select(x => new PendienteRow(tipo, x.PlantaCode, x.Code, x.UnidadNegocioCode,
                        AprobacionTipos.MonedaSoles, x.TotalPedido, x.CreatedAt, x.TrabajadorCode, x.CreatedBy))
                    .OrderByDescending(x => x.Fecha).ToList();

            case AprobacionTipos.OrdenCompra:
                return (await T.OrdenesCompra.Query().AsNoTracking()
                        .Where(x => x.Estado == EstadoOrdenCompra.Pendiente && x.TotalCompra > 0)
                        .Select(x => new { x.PlantaCode, x.Code, x.UnidadNegocioCode, x.MonedaCode, x.TotalCompra, x.CreatedAt, x.ProveedorCode, x.CreatedBy })
                        .ToListAsync(ct))
                    .Select(x => new PendienteRow(tipo, x.PlantaCode, x.Code, x.UnidadNegocioCode,
                        x.MonedaCode, x.TotalCompra, x.CreatedAt, x.ProveedorCode, x.CreatedBy))
                    .OrderByDescending(x => x.Fecha).ToList();

            case AprobacionTipos.OrdenServicio:
                return (await T.OrdenesServicio.Query().AsNoTracking()
                        .Where(x => x.Estado == EstadoOrdenServicio.Pendiente && x.Monto > 0)
                        .Select(x => new { x.PlantaCode, x.Code, x.UnidadNegocioCode, x.MonedaCode, x.Monto, x.FechaEmision, x.ProveedorCode, x.CreatedBy })
                        .ToListAsync(ct))
                    .Select(x => new PendienteRow(tipo, x.PlantaCode, x.Code, x.UnidadNegocioCode,
                        x.MonedaCode, x.Monto, x.FechaEmision, x.ProveedorCode, x.CreatedBy))
                    .OrderByDescending(x => x.Fecha).ToList();

            case AprobacionTipos.Vale:
            case AprobacionTipos.ValeCliente:
                var esCliente = tipo == AprobacionTipos.ValeCliente;
                return (await T.Vales.Query().AsNoTracking()
                        .Where(x => x.Estado == EstadoVale.Pendiente
                                 && x.Neto > 0
                                 && x.TipoValeCode != TipoValeTransferencia
                                 && (esCliente
                                     ? x.TipoValeCode == TipoValeCombustible
                                     : x.TipoValeCode != TipoValeCombustible))
                        .Select(x => new { x.PlantaCode, x.Code, x.UnidadNegocioCode, x.Neto, x.CreatedAt, x.TrabajadorCode, x.CreatedBy })
                        .ToListAsync(ct))
                    .Select(x => new PendienteRow(tipo, x.PlantaCode, x.Code, x.UnidadNegocioCode,
                        AprobacionTipos.MonedaSoles, x.Neto, x.CreatedAt, x.TrabajadorCode, x.CreatedBy))
                    .OrderByDescending(x => x.Fecha).ToList();

            default:
                throw new ArgumentException(
                    $"Tipo de transacción '{tipo}' no válido. Use: {string.Join(", ", AprobacionTipos.Todos)}.");
        }
    }

    /// <summary>Datos mínimos del documento (cualquier estado) para evaluar quién puede aprobarlo.</summary>
    public static async Task<DocumentoAprobable> CargarDocumentoAsync(
        IUnitOfWork uow,
        string tipo,
        string plantaCode,
        string code,
        CancellationToken ct)
    {
        var T = uow.Logistica.Transacciones;
        DocumentoAprobable? doc = null;

        switch (tipo)
        {
            case AprobacionTipos.Pedido:
                var p = await T.Pedidos.Query().AsNoTracking()
                    .Where(x => x.PlantaCode == plantaCode && x.Code == code)
                    .Select(x => new { x.UnidadNegocioCode, x.TotalPedido })
                    .FirstOrDefaultAsync(ct);
                if (p is not null)
                    doc = new(tipo, plantaCode, code, p.UnidadNegocioCode, AprobacionTipos.MonedaSoles, p.TotalPedido);
                break;

            case AprobacionTipos.OrdenCompra:
                var oc = await T.OrdenesCompra.Query().AsNoTracking()
                    .Where(x => x.PlantaCode == plantaCode && x.Code == code)
                    .Select(x => new { x.UnidadNegocioCode, x.MonedaCode, x.TotalCompra })
                    .FirstOrDefaultAsync(ct);
                if (oc is not null)
                    doc = new(tipo, plantaCode, code, oc.UnidadNegocioCode, oc.MonedaCode, oc.TotalCompra);
                break;

            case AprobacionTipos.OrdenServicio:
                var os = await T.OrdenesServicio.Query().AsNoTracking()
                    .Where(x => x.PlantaCode == plantaCode && x.Code == code)
                    .Select(x => new { x.UnidadNegocioCode, x.MonedaCode, x.Monto })
                    .FirstOrDefaultAsync(ct);
                if (os is not null)
                    doc = new(tipo, plantaCode, code, os.UnidadNegocioCode, os.MonedaCode, os.Monto);
                break;

            case AprobacionTipos.Vale:
            case AprobacionTipos.ValeCliente:
                var v = await T.Vales.Query().AsNoTracking()
                    .Where(x => x.PlantaCode == plantaCode && x.Code == code)
                    .Select(x => new { x.UnidadNegocioCode, x.Neto, x.TipoValeCode })
                    .FirstOrDefaultAsync(ct);
                if (v is not null)
                {
                    var tipoReal = v.TipoValeCode == TipoValeCombustible
                        ? AprobacionTipos.ValeCliente
                        : AprobacionTipos.Vale;
                    doc = new(tipoReal, plantaCode, code, v.UnidadNegocioCode, AprobacionTipos.MonedaSoles, v.Neto);
                }
                break;

            default:
                throw new ArgumentException(
                    $"Tipo de transacción '{tipo}' no válido. Use: {string.Join(", ", AprobacionTipos.Todos)}.");
        }

        return doc ?? throw new KeyNotFoundException($"Documento {tipo} {plantaCode}/{code} no encontrado.");
    }
}
