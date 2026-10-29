using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common
{
    /// <summary>
    /// Arma y valida las líneas de caja chica: autocompleta desde la Nota de Ingreso / Orden de Servicio,
    /// valida maestros, resuelve tipo de cambio y calcula parcial, impuestos y total
    /// (reglas del PowerBuilder w_reg_cajachica / dw_4).
    /// </summary>
    public static class CajaChicaLineBuilder
    {
        private static string Etiqueta(int index) => $"Línea {index + 1}";

        public static async Task<List<CajaChicaDetalle>> BuildAsync(
            IUnitOfWork uow, string cajaCode, string cajaPlantaCode,
            IReadOnlyList<CajaChicaDetalleInput> inputs, CancellationToken cancellationToken)
        {
            var lines = new List<CajaChicaDetalle>(inputs.Count);

            var control = await uow.Comunes.ControlesVentas.Query().AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            var comprobantes = new Dictionary<string, ComprobantePago?>();
            var cuentas = new Dictionary<string, CuentaContable?>();
            var igvPorFecha = new Dictionary<DateTime, decimal>();
            PlantillaAsiento? plantillaProvision = null;
            var plantillaCargada = false;
            var clavesDocumento = new HashSet<string>();

            async Task<ComprobantePago> GetComprobante(string code, string label)
            {
                if (!comprobantes.TryGetValue(code, out var comp))
                {
                    comp = await uow.Comunes.ComprobantesPago.Query().AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
                    comprobantes[code] = comp;
                }

                if (comp is null || !comp.IsActive)
                {
                    throw new InvalidOperationException($"{label}: el comprobante {code} no existe o está inactivo.");
                }

                return comp;
            }

            async Task<CuentaContable> GetCuenta(string code, string label)
            {
                if (!cuentas.TryGetValue(code, out var cuenta))
                {
                    cuenta = await uow.Contabilidad.Maestros.CuentasContables.Query().AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
                    cuentas[code] = cuenta;
                }

                if (cuenta is null || !cuenta.IsActive)
                {
                    throw new InvalidOperationException($"{label}: la cuenta contable {code} no existe o está inactiva.");
                }

                return cuenta;
            }

            async Task<decimal> GetIgvPercentage(DateTime fecha)
            {
                var day = fecha.Date;
                if (!igvPorFecha.TryGetValue(day, out var pct))
                {
                    pct = await PeriodoIgvRules.GetPorcentajeAsync(uow, day, cancellationToken);
                    igvPorFecha[day] = pct;
                }

                return pct;
            }

            async Task<string> GetCuentaCompras(string label)
            {
                if (!plantillaCargada)
                {
                    plantillaProvision = await uow.Contabilidad.Maestros.PlantillasAsiento.Query().AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Process == ProcesoAsiento.ProvisionCajaChica && p.IsActive, cancellationToken);
                    plantillaCargada = true;
                }

                var cuenta = plantillaProvision?.PurchaseAccountCode;

                if (string.IsNullOrWhiteSpace(cuenta))
                {
                    throw new InvalidOperationException(
                        $"{label}: indique la cuenta contable o configure la cuenta de compras en la plantilla de provisión de caja chica.");
                }

                return cuenta.Trim();
            }

            for (var i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                var label = Etiqueta(i);

                if (!CajaChicaRules.TryParseOrigen(input.Origin, out var origen))
                {
                    throw new InvalidOperationException($"{label}: el origen debe ser Manual (I), NotaIngreso (N) u OrdenServicio (S).");
                }

                var plantaLinea = string.IsNullOrWhiteSpace(input.PlantaCode)
                    ? cajaPlantaCode
                    : CajaChicaRules.Normalize(input.PlantaCode);

                string? comprobanteCode = input.ComprobantePagoCode;
                string? numero = input.NumeroDocumento;
                DateTime? fecha = input.FechaEmision;
                string? moneda = input.MonedaCode;
                decimal? tipoCambio = input.TipoCambio;
                decimal? cantidad = input.Cantidad;
                decimal? precio = input.PrecioUnidad;
                decimal? igvIn = input.Igv, ngrIn = input.NoGravable, renIn = input.Renta;
                decimal? fonIn = input.Fonavi, serIn = input.Servicio, iexIn = input.IgvExterior;
                string? cuentaCode = input.CuentaContableCode;
                string? proveedor = input.ProveedorCode;
                string? referencia = null;

                if (origen != OrigenLineaCajaChica.Manual)
                {
                    referencia = string.IsNullOrWhiteSpace(input.ReferenceCode)
                        ? null
                        : CajaChicaRules.Normalize(input.ReferenceCode);

                    if (referencia is null)
                    {
                        throw new InvalidOperationException($"{label}: indique el código de la nota de ingreso u orden de servicio.");
                    }

                    var yaUsada = await uow.Contabilidad.Transacciones.CajasChicaDetalle.Query()
                        .AnyAsync(d => d.CajaCode != cajaCode && d.Origen == origen
                                       && d.PlantaCode == plantaLinea && d.ReferenceCode == referencia,
                            cancellationToken);

                    if (yaUsada)
                    {
                        throw new InvalidOperationException(
                            $"{label}: {origen} {plantaLinea}-{referencia} ya está incluida en otra caja chica.");
                    }

                    if (origen == OrigenLineaCajaChica.NotaIngreso)
                    {
                        var ni = await uow.Logistica.Transacciones.NotaIngresos.Query().AsNoTracking()
                            .FirstOrDefaultAsync(n => n.PlantaCode == plantaLinea && n.Code == referencia, cancellationToken)
                            ?? throw new KeyNotFoundException($"{label}: nota de ingreso {plantaLinea}-{referencia} no encontrada.");

                        if (!string.IsNullOrWhiteSpace(ni.AsientoContable))
                        {
                            throw new InvalidOperationException(
                                $"{label}: la nota de ingreso {plantaLinea}-{referencia} ya tiene el asiento {ni.AsientoContable.Trim()}.");
                        }

                        comprobanteCode ??= ni.ComprobantePagoCode;
                        numero ??= ni.NumeroDocumento;
                        fecha ??= ni.FechaEmision;
                        moneda ??= ni.MonedaCode;
                        tipoCambio ??= ni.TipoCambio > 0 ? (decimal?)ni.TipoCambio : null;
                        cantidad ??= 1m;
                        precio ??= ni.Total;
                        igvIn ??= ni.Igv; ngrIn ??= ni.NoGravable; renIn ??= ni.Renta;
                        fonIn ??= ni.Fonavi; serIn ??= ni.Servicio; iexIn ??= ni.IgvExterior;
                        proveedor ??= ni.ProveedorCode;
                    }
                    else
                    {
                        var os = await uow.Logistica.Transacciones.OrdenesServicio.Query().AsNoTracking()
                            .FirstOrDefaultAsync(n => n.PlantaCode == plantaLinea && n.Code == referencia, cancellationToken)
                            ?? throw new KeyNotFoundException($"{label}: orden de servicio {plantaLinea}-{referencia} no encontrada.");

                        if (!string.IsNullOrWhiteSpace(os.AsientoContable))
                        {
                            throw new InvalidOperationException(
                                $"{label}: la orden de servicio {plantaLinea}-{referencia} ya tiene el asiento {os.AsientoContable.Trim()}.");
                        }

                        comprobanteCode ??= os.ComprobantePagoCode;
                        numero ??= os.NumeroDocumento;
                        fecha ??= os.FechaEmision;
                        moneda ??= os.MonedaCode;
                        tipoCambio ??= os.TipoCambio > 0 ? (decimal?)os.TipoCambio : null;
                        cantidad ??= 1m;
                        precio ??= os.Total;
                        igvIn ??= os.Igv; ngrIn ??= os.NoGravable; renIn ??= os.Renta;
                        fonIn ??= os.Fonavi; serIn ??= os.Servicio; iexIn ??= os.IgvExterior;
                        proveedor ??= os.ProveedorCode;
                    }

                    cuentaCode ??= await GetCuentaCompras(label);
                }

                // Obligatorios (manual, o lo que la nota/orden no pudo aportar)
                if (string.IsNullOrWhiteSpace(comprobanteCode)) throw new InvalidOperationException($"{label}: el comprobante es obligatorio.");
                if (string.IsNullOrWhiteSpace(numero)) throw new InvalidOperationException($"{label}: el número del comprobante es obligatorio.");
                if (!fecha.HasValue) throw new InvalidOperationException($"{label}: la fecha de emisión es obligatoria.");
                if (string.IsNullOrWhiteSpace(moneda)) throw new InvalidOperationException($"{label}: la moneda es obligatoria.");
                if (!cantidad.HasValue || cantidad.Value <= 0) throw new InvalidOperationException($"{label}: la cantidad debe ser mayor a cero.");
                if (!precio.HasValue || precio.Value < 0) throw new InvalidOperationException($"{label}: el precio no puede ser negativo.");
                if (string.IsNullOrWhiteSpace(cuentaCode)) throw new InvalidOperationException($"{label}: la cuenta contable es obligatoria.");
                if (string.IsNullOrWhiteSpace(proveedor)) throw new InvalidOperationException($"{label}: el proveedor es obligatorio.");

                var comprobanteNorm = comprobanteCode.Trim();
                var numeroNorm = numero.Trim();
                var monedaNorm = CajaChicaRules.Normalize(moneda);
                var proveedorNorm = proveedor.Trim();
                var cuentaNorm = CajaChicaRules.Normalize(cuentaCode);

                var comprobante = await GetComprobante(comprobanteNorm, label);

                var monedaOk = await uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == monedaNorm && m.IsActive, cancellationToken);
                if (!monedaOk) throw new InvalidOperationException($"{label}: la moneda {monedaNorm} no existe o está inactiva.");

                var proveedorOk = await uow.Logistica.Maestros.Proveedores.Query().AnyAsync(p => p.Code == proveedorNorm, cancellationToken);
                if (!proveedorOk) throw new InvalidOperationException($"{label}: el proveedor {proveedorNorm} no existe.");

                var plantaOk = await uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == plantaLinea, cancellationToken);
                if (!plantaOk) throw new InvalidOperationException($"{label}: la planta {plantaLinea} no existe.");

                var cuenta = await GetCuenta(cuentaNorm, label);

                // Anexo: solo si la cuenta exige tipo de anexo
                string? anexoTipo = null;
                string? anexoCode = null;
                var tipoAnexoCuenta = (cuenta.TipoAnexoCode ?? string.Empty).Trim();

                if (tipoAnexoCuenta.Length == 3)
                {
                    anexoCode = string.IsNullOrWhiteSpace(input.AnexoCode) ? null : input.AnexoCode.Trim();

                    if (anexoCode is null)
                    {
                        throw new InvalidOperationException($"{label}: la cuenta {cuentaNorm} exige un anexo ({tipoAnexoCuenta}).");
                    }

                    var anexoOk = await uow.Contabilidad.Maestros.Anexos.Query()
                        .AnyAsync(a => a.TipoAnexoCode == tipoAnexoCuenta && a.Code == anexoCode, cancellationToken);

                    if (!anexoOk)
                    {
                        throw new InvalidOperationException($"{label}: el anexo {anexoCode} no existe en el tipo {tipoAnexoCuenta}.");
                    }

                    anexoTipo = tipoAnexoCuenta;
                }

                // Nota de crédito: exige el documento que referencia
                var esNotaCredito = comprobanteNorm == AsientoNotaCreditoCode;
                string? refComprobante = null;
                string? refNumero = null;

                if (esNotaCredito)
                {
                    refComprobante = string.IsNullOrWhiteSpace(input.DocumentoReferenciaComprobantePagoCode)
                        ? null : input.DocumentoReferenciaComprobantePagoCode.Trim();
                    refNumero = string.IsNullOrWhiteSpace(input.DocumentoReferenciaNumero)
                        ? null : input.DocumentoReferenciaNumero.Trim();

                    if (refComprobante is null || refNumero is null)
                    {
                        throw new InvalidOperationException($"{label}: la nota de crédito debe indicar el comprobante y número que referencia.");
                    }

                    await GetComprobante(refComprobante, label);
                }

                // Tipo de cambio de la moneda a la fecha de emisión (1 para la moneda base)
                decimal tc;
                if (TipoCambioRules.EsMonedaBase(monedaNorm))
                {
                    tc = 1m;
                }
                else
                {
                    tc = tipoCambio ?? await TipoCambioRules.GetVentaAsync(uow, monedaNorm, fecha.Value, cancellationToken) ?? 0m;

                    if (tc <= 0)
                    {
                        throw new InvalidOperationException(
                            $"{label}: no se registra el tipo de cambio venta de {monedaNorm} para el {fecha.Value:dd/MM/yyyy}.");
                    }
                }

                // Importes: parcial y, por bandera del comprobante, impuestos (calculados si no se indican)
                var parcial = Round2(cantidad.Value * precio.Value);

                var igv = 0m;
                if (comprobante.AffectsIgv)
                {
                    igv = igvIn ?? Round2(parcial * await GetIgvPercentage(fecha.Value) / 100m);
                }

                var renta = 0m;
                var fonavi = 0m;
                var cap = control?.WithholdingCap ?? 0m;

                if (comprobante.AffectsIncomeTax)
                {
                    renta = renIn ?? (parcial > cap ? Round2(parcial * (control?.WithholdingPercentage ?? 0m) / 100m) : 0m);
                }

                if (comprobante.AffectsFonavi)
                {
                    fonavi = fonIn ?? (parcial > cap ? Round2(parcial * (control?.FonaviPercentage ?? 0m) / 100m) : 0m);
                }

                var noGravable = comprobante.IsNonTaxable ? (ngrIn ?? 0m) : 0m;
                var servicio = comprobante.IsService ? (serIn ?? 0m) : 0m;
                var igvExterior = comprobante.AffectsForeignIgv ? (iexIn ?? 0m) : 0m;

                if (igv < 0 || renta < 0 || fonavi < 0 || noGravable < 0 || servicio < 0 || igvExterior < 0)
                {
                    throw new InvalidOperationException($"{label}: los importes no pueden ser negativos.");
                }

                var total = parcial + igv - renta - fonavi + servicio + igvExterior;

                // Unicidad del comprobante (el tipo 00 queda fuera)
                if (comprobanteNorm != CajaChicaRules.ComprobanteSinDocumento)
                {
                    var clave = $"{comprobanteNorm}|{numeroNorm}|{proveedorNorm}";

                    if (!clavesDocumento.Add(clave))
                    {
                        throw new InvalidOperationException($"{label}: el comprobante {comprobanteNorm} {numeroNorm} está repetido en la caja.");
                    }

                    var enOtraCaja = await uow.Contabilidad.Transacciones.CajasChicaDetalle.Query()
                        .AnyAsync(d => d.CajaCode != cajaCode && d.ComprobantePagoCode == comprobanteNorm
                                       && d.NumeroDocumento == numeroNorm && d.ProveedorCode == proveedorNorm,
                            cancellationToken);

                    if (enOtraCaja)
                    {
                        throw new InvalidOperationException(
                            $"{label}: el comprobante {comprobanteNorm} {numeroNorm} ya está registrado en otra caja chica.");
                    }

                    var enCompras = await uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                        .AnyAsync(d => d.ComprobantePagoCode == comprobanteNorm && d.NumeroDocumento == numeroNorm
                                       && d.ProveedorCode == proveedorNorm
                                       && d.Origen != OrigenDocumentoCompra.CajaChica,
                            cancellationToken);

                    if (enCompras)
                    {
                        throw new InvalidOperationException(
                            $"{label}: el comprobante {comprobanteNorm} {numeroNorm} ya existe en el registro de compras.");
                    }
                }

                lines.Add(new CajaChicaDetalle
                {
                    CajaCode = cajaCode,
                    ItemNumber = i + 1,
                    Origen = origen,
                    ReferenceCode = referencia,
                    PlantaCode = plantaLinea,
                    ComprobantePagoCode = comprobanteNorm,
                    NumeroDocumento = numeroNorm,
                    FechaEmision = fecha.Value.Date,
                    MonedaCode = monedaNorm,
                    TipoCambio = tc,
                    Cantidad = cantidad.Value,
                    PrecioUnidad = precio.Value,
                    Parcial = parcial,
                    Igv = igv,
                    NoGravable = noGravable,
                    Renta = renta,
                    Fonavi = fonavi,
                    Servicio = servicio,
                    IgvExterior = igvExterior,
                    Total = total,
                    CuentaContableCode = cuentaNorm,
                    AnexoTipoCode = anexoTipo,
                    AnexoCode = anexoCode,
                    Concepto = string.IsNullOrWhiteSpace(input.Concepto) ? null : input.Concepto.Trim(),
                    ProveedorCode = proveedorNorm,
                    DocumentoReferenciaComprobantePagoCode = refComprobante,
                    DocumentoReferenciaNumero = refNumero,
                    DocumentoReferenciaFecha = esNotaCredito ? input.DocumentoReferenciaFecha?.Date : null
                });
            }

            return lines;
        }

        /// <summary>Código del comprobante nota de crédito (tipo de documento 07 del legacy).</summary>
        public const string AsientoNotaCreditoCode = "07";

        private static decimal Round2(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
