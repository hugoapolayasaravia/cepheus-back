using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common
{
    /// <summary>
    /// Asiento de rebote de un cheque NO diferido (legacy: co_asi_cancelacionr_x, asiento tipo 01). Reversa el
    /// asiento de la cancelación: el dinero sale del banco (Haber cuenta 10) y la deuda vuelve al cliente
    /// (Debe cuenta 12). Si el cheque es en dólares y el tipo de cambio del día del rebote es distinto al de la
    /// cancelación, la diferencia va a la cuenta de diferencia de cambio.
    ///
    /// SUPUESTOS (para confirmar): las cuentas y anexos salen de la primera línea 10 y la primera línea 12 del
    /// asiento de la cancelación (como el legacy); los importes salen del propio cheque. El legacy resta la
    /// diferencia sobre el cargo de la línea 12 de forma que no cuadra con su propio asiento; aquí la diferencia
    /// de cambio se calcula como (importe al tipo de cambio del rebote - importe al tipo de cambio original).
    /// </summary>
    public static class ChequeVentaReboteAsientoService
    {
        public const string TipoAsientoRebote = "01";
        public const string CuentaDiferenciaGanancia = "77610101"; // gs_ctadiferenciag
        public const string CuentaDiferenciaPerdida = "96110103";  // gs_ctadiferenciap

        public static async Task<AsientoContable> BuildAsync(
            IUnitOfWork uow, ChequeVenta cheque, DateTime reboteDate, CancellationToken cancellationToken)
        {
            var (oAnio, oMes, oTipo, oNumero) = AsientoContableRules.ParseCodigo(cheque.ProvisionEntryNumber!.Trim());

            var original = await uow.Contabilidad.Transacciones.AsientosContables.Query()
                .AsNoTracking()
                .Include(a => a.Detalles)
                .FirstOrDefaultAsync(
                    a => a.Anio == oAnio && a.Mes == oMes && a.TipoAsientoCode == oTipo && a.Numero == oNumero,
                    cancellationToken)
                ?? throw new InvalidOperationException(
                    $"No se encontró el asiento de la cancelación {cheque.ProvisionEntryNumber}.");

            var linea10 = original.Detalles.OrderBy(d => d.ItemNumber)
                .FirstOrDefault(d => d.CuentaContableCode.StartsWith("10"))
                ?? throw new InvalidOperationException(
                    $"El asiento {cheque.ProvisionEntryNumber} no tiene una línea de la cuenta 10 (banco/caja).");

            var linea12 = original.Detalles.OrderBy(d => d.ItemNumber)
                .FirstOrDefault(d => d.CuentaContableCode.StartsWith("12"))
                ?? throw new InvalidOperationException(
                    $"El asiento {cheque.ProvisionEntryNumber} no tiene una línea de la cuenta 12 (cuentas por cobrar).");

            var tcRebote = await DocumentoVentaRules.ResolveExchangeRateAsync(uow, cheque.MonedaCode, reboteDate, cancellationToken);

            var haberBanco = AsientoVentaService.ToSoles(cheque.TotalAmount, cheque.MonedaCode, cheque.ExchangeRate);
            var debeCliente = AsientoVentaService.ToSoles(cheque.TotalAmount, cheque.MonedaCode, tcRebote);
            var diferencia = debeCliente - haberBanco;

            var asiento = await AsientoVentaService.NewAsync(
                uow, TipoAsientoRebote, reboteDate, $"REBOTE CHEQUE NO DIFERIDO {cheque.Number}", tcRebote, cancellationToken);

            var glosa = AsientoNotaIngresoRules.Truncate($"REBOTE CHEQUE {cheque.Number}", AsientoNotaIngresoRules.DescripcionLineaMax);

            void Add(string cuenta, string? anexoTipo, string? anexoCode, decimal cargo, decimal abono)
                => asiento.Detalles.Add(new AsientoContableDetalle
                {
                    Anio = asiento.Anio,
                    Mes = asiento.Mes,
                    TipoAsientoCode = asiento.TipoAsientoCode,
                    Numero = asiento.Numero,
                    ItemNumber = asiento.Detalles.Count + 1,
                    PlantaCode = cheque.PlantaCode,
                    PlantaEjecutoraCode = cheque.PlantaCode,
                    Descripcion = glosa,
                    CuentaContableCode = cuenta,
                    AnexoTipoCode = anexoTipo,
                    AnexoCode = anexoCode,
                    NumeroDocumento = cheque.Number,
                    CargoSol = cargo,
                    AbonoSol = abono,
                    TipoContabilidad = TipoContabilidad.Todos
                });

            Add(linea10.CuentaContableCode, linea10.AnexoTipoCode, linea10.AnexoCode, 0m, haberBanco);
            Add(linea12.CuentaContableCode, linea12.AnexoTipoCode, linea12.AnexoCode, debeCliente, 0m);

            if (diferencia > 0)
            {
                Add(CuentaDiferenciaGanancia, null, null, 0m, diferencia);
            }
            else if (diferencia < 0)
            {
                Add(CuentaDiferenciaPerdida, null, null, -diferencia, 0m);
            }

            await AsientoVentaService.FinishAsync(uow, asiento, aplicarDestinos: true, cancellationToken);

            return asiento;
        }
    }
}
