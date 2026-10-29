using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common
{
    /// <summary>
    /// Asiento de provisión de la Nota de Débito (legacy: fc_genasiprovision("08", ...) / co_asipro_ventas,
    /// asiento tipo 03, "N/DEBITO X COBRAR PLANTA ...").
    ///
    /// Con plantilla (proceso ProvisionVenta):
    ///   Debe  : cuenta del cliente (línea TotalDocumento, una por moneda: 12110101 soles / 12110102 dólares).
    ///   Haber : cuenta contable de cada producto (línea DetalleDocumento, Producto.AccountingAccountCode).
    ///   Haber : IGV (línea Igv, 40110101).
    /// Los importes van en soles (importe en la moneda del documento x tipo de cambio).
    ///
    /// Fuera de alcance (el legacy solo lo usaba si Tipo_dve era P, G o C, y la ND siempre es "N"): la rama de
    /// anticipos (cuenta 12210101/12210102).
    /// </summary>
    public static class NotaDebitoVentaAsientoService
    {
        /// <summary>Tipo de comprobante de la nota de débito (SUNAT 08).</summary>
        public const string ComprobanteCode = "08";

        /// <summary>
        /// Todos los productos de la nota deben tener cuenta contable de 8 caracteres
        /// (legacy: co_asipro_ventas con flag "0", que se ejecutaba antes de imprimir).
        /// </summary>
        public static async Task<Dictionary<(string Tipo, string Producto), string>> LoadProductAccountsAsync(
            IUnitOfWork uow, NotaDebitoVenta nota, CancellationToken cancellationToken)
        {
            var accounts = new Dictionary<(string Tipo, string Producto), string>();

            foreach (var (tipo, producto) in nota.Details
                .Select(d => (d.TipoProductoCode, d.ProductoCode))
                .Distinct())
            {
                var t = tipo;
                var p = producto;

                var cuenta = await uow.Facturacion.Maestros.Productos.Query()
                    .AsNoTracking()
                    .Where(x => x.TipoProductoCode == t && x.Code == p)
                    .Select(x => x.AccountingAccountCode)
                    .FirstOrDefaultAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(cuenta) || cuenta.Trim().Length != 8)
                {
                    throw new InvalidOperationException($"Producto {t}{p} no registra cuenta contable ... Reintente ...");
                }

                accounts[(t, p)] = cuenta.Trim();
            }

            return accounts;
        }

        /// <summary>Genera (sin guardar) el asiento de provisión de la nota. Requiere Cliente y Details cargados.</summary>
        public static async Task<AsientoContable> BuildAsync(
            IUnitOfWork uow, NotaDebitoVenta nota, CancellationToken cancellationToken)
        {
            var accounts = await LoadProductAccountsAsync(uow, nota, cancellationToken);
            var plantilla = await AsientoVentaService.GetPlantillaAsync(uow, ProcesoAsiento.ProvisionVenta, cancellationToken);

            var rate = nota.ExchangeRate;
            var moneda = nota.MonedaCode;

            var lineas = nota.Details
                .OrderBy(d => d.Line)
                .Select(d => new AsientoFuenteLinea
                {
                    PlantaCode = nota.ExecutingPlantaCode,
                    Detalle = AsientoNotaIngresoRules.Truncate(d.Description, AsientoNotaIngresoRules.DescripcionLineaMax),
                    CuentaCode = accounts[(d.TipoProductoCode, d.ProductoCode)],
                    Parcial = AsientoVentaService.ToSoles(d.Total, moneda, rate)
                })
                .ToList();

            var igv = AsientoVentaService.ToSoles(nota.IgvAmount, moneda, rate);

            var fuente = new AsientoFuente
            {
                PlantaCode = nota.ExecutingPlantaCode,
                Glosa = AsientoNotaIngresoRules.Truncate(
                    $"N/DEBITO X COBRAR PLANTA {nota.ExecutingPlantaCode}", AsientoNotaIngresoRules.DescripcionLineaMax),
                NumeroDocumento = nota.Number,
                ComprobantePagoCode = ComprobanteCode,
                EsNotaCredito = false,
                MonedaCode = moneda,
                ClienteCode = nota.ClienteCode,
                // El total se arma con las líneas ya convertidas para que el asiento cuadre al céntimo.
                Total = lineas.Sum(l => l.Parcial) + igv,
                Igv = igv,
                Lineas = lineas
            };

            return await AsientoVentaService.BuildAsync(
                uow, plantilla, new[] { fuente }, nota.IssueDate, nota.Cliente?.Name ?? nota.ClienteCode,
                rate, aplicarDestinos: false, cancellationToken);
        }
    }
}
