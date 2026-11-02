using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common
{
    /// <summary>
    /// Arma el asiento de cancelación del pago (legacy fc_genasicanprv + co_genera_destino) con la
    /// plantilla del proceso CancelacionProveedor. Todo en soles (tipo de cambio 1).
    ///
    /// Reglas del legacy que conserva:
    ///   - Un asiento por pago: descripción "CANCELA PROVEEDORES {código del pago}", fecha de proceso =
    ///     fecha de emisión del pago, origen Logística, estado Pendiente.
    ///   - Una línea por documento: la cuenta, la planta y el anexo salen de la línea del asiento de
    ///     provisión del documento (cuenta que empieza igual que la cuenta por pagar del proveedor, con el
    ///     proveedor como anexo y el número del documento). La nota de crédito va al lado contrario.
    ///   - Retención del IGV solo si es mayor a 0, con el número del comprobante de retención.
    ///   - Total a pagar en la cuenta contable de la cuenta bancaria; si esa cuenta exige tipo de anexo, el
    ///     anexo es el proveedor. El documento de la línea es el número de cheque.
    ///   - Los destinos se generan al final (co_genera_destino).
    /// </summary>
    public static class AsientoCancelacionProveedorGenerator
    {
        public static async Task<PlantillaAsiento> GetPlantillaAsync(IUnitOfWork uow, CancellationToken cancellationToken)
            => await uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .AsNoTracking()
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Process == ProcesoAsiento.CancelacionProveedor && p.IsActive, cancellationToken)
               ?? throw new InvalidOperationException(
                   $"No existe una plantilla de asiento activa para el proceso {ProcesoAsiento.CancelacionProveedor}.");

        public static async Task<AsientoContable> BuildAsync(
            IUnitOfWork uow,
            PagoProveedor pago,
            Proveedor proveedor,
            PlantillaAsiento plantilla,
            CancellationToken cancellationToken)
        {
            var anio = pago.FechaEmision.Year.ToString("0000");
            var mes = pago.FechaEmision.Month.ToString("00");

            await uow.AcquireAppLockAsync(
                AsientoNumeroGenerator.LockResource(anio, mes, plantilla.TipoAsientoCode), 15000, cancellationToken);
            var numero = await AsientoNumeroGenerator.NextAsync(uow, anio, mes, plantilla.TipoAsientoCode, cancellationToken);

            var asiento = new AsientoContable
            {
                Anio = anio,
                Mes = mes,
                TipoAsientoCode = plantilla.TipoAsientoCode,
                Numero = numero,
                Descripcion = $"CANCELA PROVEEDORES {pago.Code}",
                FechaProceso = pago.FechaEmision.Date,
                TipoCambio = 1m,
                Estado = EstadoAsiento.Pendiente,
                Origen = OrigenAsiento.Logistica
            };

            var cuentaPorPagar = (await ProveedorCuentaPorPagarResolver.ResolverAsync(
                uow, proveedor.Code, PagoProveedorRules.MonedaBase, cancellationToken)).Trim();
            if (cuentaPorPagar.Length < 4)
            {
                throw new InvalidOperationException(
                    $"Cuenta por pagar del proveedor {proveedor.Code} incorrecta ... Reintente ...");
            }
            var prefijo = cuentaPorPagar[..4];

            var nombre = AsientoNotaIngresoRules.Truncate(proveedor.LegalName, AsientoNotaIngresoRules.DescripcionLineaMax);
            var item = 0;
            string? plantaUltima = null;

            foreach (var tl in plantilla.Lines.OrderBy(l => l.Order))
            {
                switch (tl.Concept)
                {
                    case ConceptoAsiento.PagoDocumento:
                        foreach (var linea in pago.Detalles.OrderBy(d => d.Id))
                        {
                            var provision = await BuscarLineaProvisionAsync(
                                uow, linea, prefijo, proveedor.Code, cancellationToken);

                            var naturaleza = tl.Nature;
                            if (tl.InvertsOnCreditNote && linea.ComprobantePagoCode == PagoProveedorRules.NotaCredito)
                            {
                                naturaleza = naturaleza == NaturalezaAsiento.Debe ? NaturalezaAsiento.Haber : NaturalezaAsiento.Debe;
                            }

                            plantaUltima = provision.PlantaCode;
                            asiento.Detalles.Add(NuevaLinea(
                                asiento, ++item, tl, naturaleza, Math.Abs(linea.PagoSol),
                                provision.PlantaCode, provision.CuentaContableCode,
                                provision.AnexoTipoCode, provision.AnexoCode,
                                nombre, linea.NumeroDocumento, linea.ComprobantePagoCode, provision.PlantaCode));
                        }
                        break;

                    case ConceptoAsiento.PagoRetencion:
                        if (pago.RetencionSol > 0)
                        {
                            var cuentaRetencion = (tl.AccountCode ?? string.Empty).Trim();
                            await EnsureCuentaAsync(uow, cuentaRetencion, cancellationToken);

                            asiento.Detalles.Add(NuevaLinea(
                                asiento, ++item, tl, tl.Nature, pago.RetencionSol,
                                plantaUltima ?? pago.PlantaCode, cuentaRetencion, null, null,
                                nombre, pago.NumeroComprobanteRetencion, null, plantaUltima ?? pago.PlantaCode));
                        }
                        break;

                    case ConceptoAsiento.PagoTotal:
                        var cuentaBanco = await EnsureCuentaAsync(uow, pago.CuentaContableCode.Trim(), cancellationToken);
                        if (cuentaBanco.Trim().Length != 8)
                        {
                            throw new InvalidOperationException(
                                $"Cuenta y Banco {pago.BancoCode} {pago.CuentaBancariaNumero} no registra cuenta ... Reintente ...");
                        }

                        string? anexoTipo = null;
                        string? anexoCode = null;
                        var tipoCuenta = (await TipoAnexoDeCuentaAsync(uow, pago.CuentaContableCode.Trim(), cancellationToken)) ?? string.Empty;
                        if (tl.AnnexSource == OrigenAnexoAsiento.Proveedor && tipoCuenta.Trim().Length == 3)
                        {
                            anexoTipo = tipoCuenta.Trim();
                            anexoCode = proveedor.Code;
                            var existe = await uow.Contabilidad.Maestros.Anexos.Query().AsNoTracking()
                                .AnyAsync(a => a.TipoAnexoCode == anexoTipo && a.Code == anexoCode, cancellationToken);
                            if (!existe)
                            {
                                throw new InvalidOperationException(
                                    $"El anexo {anexoCode} no está registrado en el tipo de anexo {anexoTipo}.");
                            }
                        }

                        asiento.Detalles.Add(NuevaLinea(
                            asiento, ++item, tl, tl.Nature, pago.TotalSol,
                            pago.PlantaCode, pago.CuentaContableCode.Trim(), anexoTipo, anexoCode,
                            nombre, string.IsNullOrWhiteSpace(pago.NumeroCheque) ? null : pago.NumeroCheque.Trim(),
                            null, pago.PlantaCode));
                        break;
                }
            }

            AsientoCuadre.Totalize(asiento);
            await AsientoDestinoGenerator.ApplyAsync(uow, asiento, cancellationToken);
            AsientoCuadre.EnsureBalanced(asiento);

            await uow.Contabilidad.Transacciones.AsientosContables.AddAsync(asiento, cancellationToken);
            return asiento;
        }

        private static AsientoContableDetalle NuevaLinea(
            AsientoContable asiento, int item, PlantillaAsientoLinea tl, NaturalezaAsiento naturaleza, decimal importe,
            string plantaCode, string cuentaCode, string? anexoTipo, string? anexoCode,
            string descripcion, string? numeroDocumento, string? comprobantePagoCode, string plantaEjecutora)
            => new()
            {
                Anio = asiento.Anio,
                Mes = asiento.Mes,
                TipoAsientoCode = asiento.TipoAsientoCode,
                Numero = asiento.Numero,
                ItemNumber = item,
                PlantaCode = plantaCode,
                EsDestino = tl.IsDestination,
                Descripcion = descripcion,
                CuentaContableCode = cuentaCode,
                AnexoTipoCode = anexoTipo,
                AnexoCode = anexoCode,
                NumeroDocumento = string.IsNullOrWhiteSpace(numeroDocumento) ? null : numeroDocumento.Trim(),
                CargoSol = naturaleza == NaturalezaAsiento.Debe ? importe : 0m,
                AbonoSol = naturaleza == NaturalezaAsiento.Haber ? importe : 0m,
                TipoContabilidad = tl.AccountingType,
                PlantaEjecutoraCode = plantaEjecutora,
                ComprobantePagoCode = comprobantePagoCode
            };

        /// <summary>
        /// Legacy: busca en el asiento de provisión del documento la línea de la cuenta por pagar del proveedor
        /// (cuenta que empieza igual, anexo = proveedor, mismo número de documento).
        /// </summary>
        private static async Task<AsientoContableDetalle> BuscarLineaProvisionAsync(
            IUnitOfWork uow, PagoProveedorDetalle linea, string prefijoCuenta, string proveedorCode,
            CancellationToken cancellationToken)
        {
            var codigo = (linea.AsientoProvisionCode ?? string.Empty).Trim();
            if (codigo.Length != PagoProveedorRules.AsientoLength)
            {
                throw new InvalidOperationException(
                    $"No se registra asiento de provisión del documento {linea.ComprobantePagoCode} {linea.NumeroDocumento}.");
            }

            var anio = codigo[..4];
            var mes = codigo.Substring(4, 2);
            var tipo = codigo.Substring(6, 2);
            var numero = codigo.Substring(8, 5);
            var documento = linea.NumeroDocumento.Trim();

            var provision = await uow.Contabilidad.Transacciones.AsientoContableDetalles.Query()
                .AsNoTracking()
                .Where(d => d.Anio == anio && d.Mes == mes && d.TipoAsientoCode == tipo && d.Numero == numero
                            && d.CuentaContableCode.StartsWith(prefijoCuenta)
                            && d.AnexoCode == proveedorCode
                            && d.NumeroDocumento == documento)
                .OrderBy(d => d.ItemNumber)
                .FirstOrDefaultAsync(cancellationToken);

            return provision ?? throw new InvalidOperationException(
                $"El asiento de provisión {codigo} no registra datos de la cuenta {prefijoCuenta} ni el anexo {proveedorCode}.");
        }

        private static async Task<string> EnsureCuentaAsync(IUnitOfWork uow, string code, CancellationToken cancellationToken)
        {
            var cuenta = await uow.Contabilidad.Maestros.CuentasContables.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

            if (cuenta is null)
            {
                throw new InvalidOperationException($"Cuenta {code} no registrada ... Reintente ...");
            }

            return cuenta.Code;
        }

        private static async Task<string?> TipoAnexoDeCuentaAsync(IUnitOfWork uow, string code, CancellationToken cancellationToken)
            => await uow.Contabilidad.Maestros.CuentasContables.Query()
                .AsNoTracking()
                .Where(c => c.Code == code)
                .Select(c => c.TipoAnexoCode)
                .FirstOrDefaultAsync(cancellationToken);
    }
}
