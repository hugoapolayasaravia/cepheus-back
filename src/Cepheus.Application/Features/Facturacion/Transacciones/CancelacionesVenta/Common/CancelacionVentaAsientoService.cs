using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common
{
    /// <summary>
    /// Asiento contable de la cancelación (legacy: fc_genasicancelacion, asiento tipo gs_tipocancelacioncli = 07),
    /// armado con la plantilla activa del proceso CancelacionVenta.
    ///
    /// Este servicio calcula los datos y la plantilla define las cuentas, la naturaleza y el tipo de asiento:
    ///   - DetalleDocumento: por cada documento cancelado, la cuenta y el anexo de su línea de cuentas por cobrar en su
    ///     asiento de provisión (facturas, boletas, NC y ND: cuenta 1211; letras: 1231; cheques: la cuenta 12 de su
    ///     asiento; cobranza dudosa: 1291). Va al Haber por lo pagado; la nota de crédito se invierte sola.
    ///   - PagoRecibido: el total de la cancelación en soles, en la cuenta de la cuenta bancaria de depósito o, para
    ///     letras (03 y 11), en la cuenta del tipo de pago por moneda. El canje (04) no mueve dinero y no lo genera.
    ///   - CobranzaDudosa: lo cobrado de documentos dudosos (Debe 19210101 / Haber 76410101 en el legacy).
    ///   - DiferenciaCambioGanancia / DiferenciaCambioPerdida: lo que falte para cuadrar (77610101 / 96110103 en el legacy).
    ///
    /// SUPUESTOS (para confirmar): las líneas de provisión se buscan por cuenta y anexo del cliente; si el anexo no
    /// coincide se toma la primera línea con esa cuenta. El legacy llama a fc_genasicancelacion siempre con diferido = 'N',
    /// por eso nunca usa las cuentas de diferido (12120201 / 12120202) y aquí tampoco. No guarda: quien llama hace el
    /// SaveChanges.
    /// </summary>
    public static class CancelacionVentaAsientoService
    {
        public static async Task<AsientoContable> BuildAsync(
            IUnitOfWork uow, CancelacionVenta c, CancellationToken cancellationToken)
        {
            var lines = c.Details.OrderBy(d => d.Item).ToList();

            if (lines.Count == 0)
            {
                throw new InvalidOperationException($"La cancelación {c.Code} no registra documentos.");
            }

            // Verifico que todos los documentos tengan asiento de provisión.
            foreach (var l in lines)
            {
                if (!CancelacionVentaRules.IsAsiento(l.ProvisionEntryNumber))
                {
                    throw new InvalidOperationException(
                        $"Documento {l.DocumentNumber} no registra asiento de provisión ({l.TypeCode}).");
                }
            }

            var plantilla = await AsientoVentaService.GetPlantillaAsync(uow, ProcesoAsiento.CancelacionVenta, cancellationToken);

            var provisions = new Dictionary<string, AsientoContable>();

            async Task<AsientoContable> GetProvisionAsync(string code)
            {
                if (provisions.TryGetValue(code, out var cached))
                {
                    return cached;
                }

                var (anio, mes, tipo, numero) = AsientoContableRules.ParseCodigo(code);

                var a = await uow.Contabilidad.Transacciones.AsientosContables.Query()
                    .AsNoTracking()
                    .Include(x => x.Detalles)
                    .FirstOrDefaultAsync(
                        x => x.Anio == anio && x.Mes == mes && x.TipoAsientoCode == tipo && x.Numero == numero,
                        cancellationToken)
                    ?? throw new InvalidOperationException($"No se encontró el asiento de provisión {code}.");

                provisions[code] = a;
                return a;
            }

            static AsientoContableDetalle? FindLine(AsientoContable provision, string prefix, string cliente)
            {
                var candidates = provision.Detalles.OrderBy(d => d.ItemNumber)
                    .Where(d => d.CuentaContableCode.StartsWith(prefix)).ToList();

                return candidates.FirstOrDefault(d => d.AnexoCode == cliente) ?? candidates.FirstOrDefault();
            }

            var cargos = 0m;
            var abonos = 0m;
            var dudosa = 0m;
            var fuenteLineas = new List<AsientoFuenteLinea>();

            // ---- Documentos cancelados
            foreach (var l in lines)
            {
                var provision = await GetProvisionAsync(l.ProvisionEntryNumber!.Trim());

                string prefix;
                switch (l.TypeCode)
                {
                    case CobranzaTipoDocumento.Letra:
                        prefix = "1231";
                        break;
                    case CobranzaTipoDocumento.Cheque:
                        // Solo se cancelan cheques rebotados: la cuenta 12 es la de su asiento.
                        var l12 = FindLine(provision, "12", c.ClienteCode)
                            ?? throw new InvalidOperationException(
                                $"Asiento {l.ProvisionEntryNumber} no registrado para cuenta 12 del cliente {c.ClienteCode}.");
                        prefix = l12.CuentaContableCode[..4];
                        break;
                    default:
                        prefix = "1211";
                        break;
                }

                var esNotaCredito = l.TypeCode == CobranzaTipoDocumento.NotaCredito;

                if (l.IsDoubtful)
                {
                    prefix = "1291";
                    // Igual que el legacy: la nota de crédito se resta de su importe (que ya viene en negativo).
                    dudosa += esNotaCredito ? -l.PaidSoles : l.PaidSoles;
                }

                var pl = FindLine(provision, prefix, c.ClienteCode)
                    ?? throw new InvalidOperationException(
                        $"Asiento {l.ProvisionEntryNumber} no registrado para cuenta {prefix} del cliente {c.ClienteCode}.");

                var importe = Math.Abs(l.PaidSoles);

                if (esNotaCredito)
                {
                    cargos += importe;
                }
                else
                {
                    abonos += importe;
                }

                fuenteLineas.Add(new AsientoFuenteLinea
                {
                    PlantaCode = pl.PlantaCode,
                    Detalle = pl.Descripcion,
                    CuentaCode = pl.CuentaContableCode,
                    AnexoTipoCode = pl.AnexoTipoCode,
                    AnexoCode = pl.AnexoCode,
                    Parcial = importe,
                    NumeroDocumento = l.DocumentNumber,
                    InvierteNaturaleza = esNotaCredito
                });
            }

            // ---- Caja / banco
            var cuentaPago = await ResolveCajaCuentaAsync(uow, c, cancellationToken);
            var totalPago = cuentaPago is null ? 0m : AsientoVentaService.ToSoles(c.TotalAmount, c.MonedaCode, c.ExchangeRate);

            cargos += totalPago;

            var documentoPago = c.PaymentNumber;
            if (c.TipoPagoCode == CancelacionTipoPago.LetrasVarias)
            {
                documentoPago = await uow.Facturacion.Transacciones.LetrasVenta.Query().AsNoTracking()
                    .Where(x => x.CancelacionCode == c.Code).OrderBy(x => x.Number)
                    .Select(x => x.Number).FirstOrDefaultAsync(cancellationToken);
            }

            if (documentoPago is { Length: > 10 })
            {
                documentoPago = documentoPago[^10..];
            }

            // ---- Cobranza dudosa (una línea al Debe y otra al Haber por el mismo importe)
            if (dudosa > 0)
            {
                cargos += dudosa;
                abonos += dudosa;
            }

            // ---- Diferencia de cambio: lo que falte para cuadrar.
            var diferencia = DocumentoVentaRules.Round2(cargos) - DocumentoVentaRules.Round2(abonos);

            var clienteName = await uow.Facturacion.Maestros.Clientes.Query().AsNoTracking()
                .Where(x => x.Code == c.ClienteCode).Select(x => x.Name).FirstOrDefaultAsync(cancellationToken)
                ?? c.ClienteCode;

            var fuente = new AsientoFuente
            {
                PlantaCode = c.PlantaCode,
                Glosa = clienteName,
                NumeroDocumento = documentoPago,
                MonedaCode = c.MonedaCode,
                ClienteCode = c.ClienteCode,
                CuentaPagoCode = cuentaPago,
                TotalPago = totalPago,
                CobranzaDudosa = dudosa > 0 ? dudosa : 0m,
                DiferenciaGanancia = diferencia > 0 ? diferencia : 0m,
                DiferenciaPerdida = diferencia < 0 ? -diferencia : 0m,
                Lineas = fuenteLineas
            };

            return await AsientoVentaService.BuildAsync(
                uow, plantilla, new[] { fuente }, c.Date, $"CANCELA DOCUMENTOS POR TIPO {c.TipoPagoCode}",
                c.ExchangeRate, aplicarDestinos: true, cancellationToken);
        }

        /// <summary>
        /// Cuenta contable de la caja o el banco donde entra el pago (legacy: rama "Busco Cuentas por tipo de Pago").
        /// Devuelve null para el canje (04), que no mueve dinero.
        /// </summary>
        private static async Task<string?> ResolveCajaCuentaAsync(
            IUnitOfWork uow, CancelacionVenta c, CancellationToken cancellationToken)
        {
            if (c.TipoPagoCode == CancelacionTipoPago.Canje)
            {
                return null;
            }

            async Task<string?> TipoPagoCuentaAsync()
                => await uow.Facturacion.Catalogos.TiposPago.Query().AsNoTracking()
                    .Where(t => t.Code == c.TipoPagoCode)
                    .SelectMany(t => t.Accounts)
                    .Where(a => a.MonedaCode == c.MonedaCode)
                    .Select(a => a.AccountCode)
                    .FirstOrDefaultAsync(cancellationToken);

            if (c.TipoPagoCode is CancelacionTipoPago.Letras or CancelacionTipoPago.LetrasVarias)
            {
                return await TipoPagoCuentaAsync()
                    ?? throw new InvalidOperationException(
                        $"Tipo de pago {c.TipoPagoCode} no registra cuentas para la moneda {c.MonedaCode}.");
            }

            if (!string.IsNullOrWhiteSpace(c.BancoDepCode) && !string.IsNullOrWhiteSpace(c.AccountNumber))
            {
                var banco = c.BancoDepCode.Trim();
                var cuenta = c.AccountNumber.Trim();

                var account = await uow.Contabilidad.Maestros.CuentasBancarias.Query().AsNoTracking()
                    .Where(x => x.BankCode == banco && x.AccountNumber == cuenta && x.CurrencyCode == c.MonedaCode)
                    .Select(x => x.AccountCode)
                    .FirstOrDefaultAsync(cancellationToken);

                if (account is not null)
                {
                    return account;
                }
            }

            return await TipoPagoCuentaAsync()
                ?? throw new InvalidOperationException(
                    $"Tipo de pago {c.TipoPagoCode} no registra cuentas: indique una cuenta bancaria de depósito o cargue las cuentas del tipo de pago.");
        }
    }
}
