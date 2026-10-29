using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;

/// <summary>Resultado del cálculo de la línea de crédito de un cliente/obra.</summary>
public sealed record CreditoSnapshot(string Mode, decimal Limit, decimal Used, decimal Available, string? Note);

/// <summary>
/// Línea de crédito (PB: UF_CRE_LINEA_CREDITO -> test -> fn_linea_credito_cliente / fn_linea_credito_cliente_obra).
/// Disponible = límite - utilizado:
///  - Cliente con línea global: límite = monto global; con forma de pago NO crédito (contado) el disponible es 0.
///  - Sin línea global: límite = crédito de la obra (Obra.CreditLimit), utilizado a nivel de obra.
///  - Obra de proyecto: disponible 0.
/// Se quitó la excepción fija del cliente 04221 (era la propia empresa); si se pide, se agrega como parámetro.
/// </summary>
public static class OrdenTrabajoVentaCredito
{
    public static async Task<CreditoSnapshot> GetAsync(
        IUnitOfWork uow,
        Cliente cliente,
        Obra obra,
        CancellationToken ct)
    {
        if (obra.IsProject)
        {
            return new CreditoSnapshot("Proyecto", 0m, 0m, 0m, "Obra de proyecto: sin línea de crédito.");
        }

        if (cliente.HasGlobalCreditLine)
        {
            var limit = cliente.GlobalCreditAmount ?? 0m;

            if (cliente.FormaPagoVenta is { IsCredit: false })
            {
                return new CreditoSnapshot("Global", limit, 0m, 0m, "Cliente con forma de pago al contado: sin crédito disponible.");
            }

            var used = await GetUsedAsync(uow, cliente.Code, null, ct);

            return new CreditoSnapshot("Global", limit, used, limit - used, null);
        }

        var usedObra = await GetUsedAsync(uow, cliente.Code, obra.Code, ct);

        return new CreditoSnapshot("Obra", obra.CreditLimit, usedObra, obra.CreditLimit - usedObra, null);
    }

    /// <summary>Factor fijo que usa la vista vw_ctacteot_final sobre el total de la OT (Total_ped * 1.19).</summary>
    private const decimal OrderTotalFactor = 1.19m;

    /// <summary>Moneda de las OT que cuentan: la vista solo suma PENSOL (soles); las OT en dólares no entran.</summary>
    private const string SolesCurrencyCode = "S";

    /// <summary>
    /// Cuándo se valida al grabar las líneas (PB: ot_ventas, w_reg_lim_cre_ot): OT que no es Descuento, cliente no VIP
    /// que "Requiere aprobación O.T.", sin aprobación especial de Créditos ya otorgada, obra a crédito y que no es de
    /// proyecto.
    /// </summary>
    public static bool IsRequired(OrdenTrabajoVenta ot, Cliente cliente, Obra obra)
        => ot.Type != TipoOrdenTrabajoVenta.Descuento
           && !cliente.IsVip
           && cliente.RequiresWorkOrderApproval == true
           && !ot.SpecialApprovalGranted
           && !obra.IsProject
           && (obra.FormaPagoVenta ?? cliente.FormaPagoVenta)?.IsCredit == true;

    /// <summary>Falla si el total de la OT excede el crédito disponible (o este ya está negativo).</summary>
    public static async Task EnsureCoveredAsync(
        IUnitOfWork uow,
        OrdenTrabajoVenta ot,
        Cliente cliente,
        Obra obra,
        CancellationToken ct)
    {
        if (!IsRequired(ot, cliente, obra))
        {
            return;
        }

        var credito = await GetAsync(uow, cliente, obra, ct);

        if (credito.Available < 0m || ot.Total > credito.Available)
        {
            throw new InvalidOperationException(
                $"La línea de crédito disponible ({credito.Available:N2}) no cubre el total de la OT ({ot.Total:N2}). " +
                "Solicite la aprobación especial a Créditos.");
        }
    }

    /// <summary>
    /// Utilizado según vw_ctacteot_final / VW_CtaCteOT: OT en soles, Activas, en etapa C o G, con aprobación a Crédito,
    /// por Total * 1.19. Falta restar (cuando existan) las facturas/boletas emitidas y las guías despachadas sin documento,
    /// y sumar boletas, facturas, letras, guías sin facturar, notas de crédito y débito pendientes.
    /// </summary>
    private static async Task<decimal> GetUsedAsync(IUnitOfWork uow, string clienteCode, string? obraCode, CancellationToken ct)
    {
        var query = uow.Facturacion.Transacciones.OrdenesTrabajoVenta
            .Query().AsNoTracking()
            .Where(o => o.ClienteCode == clienteCode
                        && o.CurrencyCode == SolesCurrencyCode
                        && o.Status == EstadoOrdenTrabajoVenta.Activo
                        && (o.Stage == EtapaOrdenTrabajoVenta.AprobadaCredito
                            || o.Stage == EtapaOrdenTrabajoVenta.GeneraProgramacion)
                        && o.Aprobacion != null
                        && o.Aprobacion.PaymentType == TipoPagoAprobacion.Credito);

        if (obraCode is not null)
        {
            query = query.Where(o => o.ObraCode == obraCode);
        }

        var totals = await query.Select(o => o.Total).ToListAsync(ct);

        return totals.Sum(t => Math.Round(t * OrderTotalFactor, 2));
    }
}
