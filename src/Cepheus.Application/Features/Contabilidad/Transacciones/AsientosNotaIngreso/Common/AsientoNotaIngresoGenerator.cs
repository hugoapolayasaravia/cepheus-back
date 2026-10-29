using System.Data;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Logistica.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common
{
    /// <summary>
    /// Generación del asiento contable de una nota de ingreso (legacy: eventos ue_generaasiento /
    /// ue_generaasientoind / ue_transferencia de w_con_asientosingreso y la función fc_genasiprocompras),
    /// ahora en UNA transacción y guiada por la plantilla de asiento activa del proceso.
    ///
    /// Proceso ProvisionCompraNotaIngreso (nota de ingreso por compra o importación):
    ///   1. Crea el documento de compra desde la nota de ingreso (origen Logística).
    ///   2. Arma el asiento de provisión con la plantilla, genera los destinos y exige que cuadre.
    ///   3. Guarda el código del asiento en el documento y en la nota de ingreso.
    /// Proceso TransferenciaNotaIngreso (nota de ingreso por transferencia entre plantas): asiento directo con la
    /// plantilla, sin documento de compra ni destinos (igual que el legacy).
    /// </summary>
    public static class AsientoNotaIngresoGenerator
    {
        public static Task<AsientoNotaIngresoResultResponse> GenerateAsync(
            IUnitOfWork uow, string plantaCode, string notaIngresoCode, CancellationToken cancellationToken)
        {
            var planta = plantaCode.Trim().ToUpperInvariant();
            var codigo = notaIngresoCode.Trim().ToUpperInvariant();

            return uow.ExecuteInTransactionAsync(
                token => GenerateCoreAsync(uow, planta, codigo, token),
                IsolationLevel.ReadCommitted,
                cancellationToken);
        }

        private static async Task<decimal?> GetVentaAsync(IUnitOfWork uow, DateTime fecha, CancellationToken ct)
        {
            var date = DateOnly.FromDateTime(fecha);
            return await uow.Comunes.TiposCambio.Query()
                .AsNoTracking()
                .Where(t => t.Date == date)
                .Select(t => (decimal?)t.SellRate)
                .FirstOrDefaultAsync(ct);
        }

        private static async Task<AsientoNotaIngresoResultResponse> GenerateCoreAsync(
            IUnitOfWork uow, string plantaCode, string niCode, CancellationToken ct)
        {
            var ni = await uow.Logistica.Transacciones.NotaIngresos.Query()
                .Include(n => n.Proveedor)
                .Include(n => n.ComprobantePago)
                .Include(n => n.FormaPago)
                .Include(n => n.Detalles).ThenInclude(d => d.Articulo)
                .FirstOrDefaultAsync(n => n.PlantaCode == plantaCode && n.Code == niCode, ct)
                ?? throw new KeyNotFoundException($"Nota de ingreso {plantaCode}-{niCode} no encontrada.");

            var etiqueta = $"{ni.PlantaCode}-{ni.Code}";

            if (ni.Estado != EstadoNotaIngreso.Procesado)
            {
                throw new InvalidOperationException(
                    $"La nota de ingreso {etiqueta} está en estado {ni.Estado}; solo se genera asiento de notas de ingreso Procesadas.");
            }

            if (!string.IsNullOrWhiteSpace(ni.AsientoContable))
            {
                throw new InvalidOperationException(
                    $"La nota de ingreso {etiqueta} ya tiene el asiento {ni.AsientoContable.Trim()}.");
            }

            if (ni.ComprobantePago is null || !ni.ComprobantePago.IsActive)
            {
                throw new InvalidOperationException(
                    $"El comprobante de pago {ni.ComprobantePagoCode} de la nota de ingreso {etiqueta} no existe o está inactivo.");
            }

            var esTransferencia = ni.Origen == OrigenNotaIngreso.Transferencia;
            var proceso = esTransferencia
                ? ProcesoAsiento.TransferenciaNotaIngreso
                : ProcesoAsiento.ProvisionCompraNotaIngreso;

            var plantilla = await uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .AsNoTracking()
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Process == proceso && p.IsActive, ct)
                ?? throw new InvalidOperationException(
                    $"No existe una plantilla de asiento activa para el proceso {proceso}.");

            var fechaRecepcion = ni.FechaRecepcion.Date;
            var anio = fechaRecepcion.Year.ToString("0000");
            var mes = fechaRecepcion.Month.ToString("00");

            await PeriodoContableRules.EnsureOpenAsync(uow, anio, mes, ct);

            var glosa = $"{ni.PlantaCode} Nota de Ingreso {ni.Code}";
            var esSoles = string.Equals(ni.MonedaCode, DocumentoCompraMonedaSoles, StringComparison.OrdinalIgnoreCase);

            DocumentoCompra? documento = null;
            AsientoFuente fuente;
            decimal tipoCambioAsiento;

            if (esTransferencia)
            {
                var venta = await GetVentaAsync(uow, ni.FechaEmision, ct) ?? 0m;
                if (venta <= 0)
                {
                    throw new InvalidOperationException($"Tipo de Cambio es cero {etiqueta} ... Verifique ...");
                }

                tipoCambioAsiento = venta;

                decimal ToSolesTransferencia(decimal x) =>
                    esSoles ? x : Math.Round(x * venta, 2, MidpointRounding.AwayFromZero);

                fuente = new AsientoFuente
                {
                    PlantaCode = ni.PlantaCode,
                    Glosa = glosa,
                    NumeroDocumento = ni.NumeroDocumento,
                    ComprobantePagoCode = ni.ComprobantePagoCode,
                    EsNotaCredito = false,
                    MontoNotaIngreso = ToSolesTransferencia(ni.Monto),
                    NetoNotaIngreso = ToSolesTransferencia(ni.Total)
                };
            }
            else
            {
                var resultadoVinculo = await TryLinkExistingAsync(uow, ni, etiqueta, ct);
                if (resultadoVinculo is not null)
                {
                    return resultadoVinculo;
                }

                var venta = await GetVentaAsync(uow, ni.FechaEmision, ct);
                var tca = esSoles ? (venta ?? ni.TipoCambio) : ni.TipoCambio;
                if (tca <= 0)
                {
                    throw new InvalidOperationException($"Tipo de Cambio es cero {etiqueta} ... Verifique ...");
                }

                tipoCambioAsiento = tca;

                documento = await BuildDocumentoAsync(uow, ni, plantilla.PurchaseAccountCode, glosa, esSoles, tca, ct);

                var cuentaPorPagar = plantilla.Lines.Any(l => l.AccountSource == OrigenCuentaAsiento.CuentaPorPagarProveedor)
                    ? await ProveedorCuentaPorPagarResolver.ResolverAsync(uow, ni.ProveedorCode, ni.MonedaCode, ct)
                    : null;

                fuente = new AsientoFuente
                {
                    PlantaCode = ni.PlantaCode,
                    Glosa = glosa,
                    NumeroDocumento = documento.NumeroDocumento,
                    ComprobantePagoCode = documento.ComprobantePagoCode,
                    EsNotaCredito = documento.ComprobantePagoCode == AsientoNotaIngresoRules.NotaCreditoComprobanteCode,
                    ProveedorCode = ni.ProveedorCode,
                    ProveedorNombre = ni.Proveedor?.LegalName,
                    CuentaPorPagarCode = cuentaPorPagar,
                    Total = documento.TotalSol,
                    Igv = documento.IgvSol,
                    IgvExterior = documento.IgvExteriorSol,
                    Renta = documento.RentaSol,
                    Fonavi = documento.FonaviSol,
                    NoGravable = documento.NoGravableSol,
                    Servicio = documento.ServicioSol,
                    Lineas = documento.Detalles
                        .OrderBy(d => d.ItemNumber)
                        .Select(d => new AsientoFuenteLinea
                        {
                            PlantaCode = d.PlantaCode,
                            Detalle = d.Detalle,
                            CuentaCode = d.CuentaContableCode,
                            AnexoTipoCode = d.AnexoTipoCode,
                            AnexoCode = d.AnexoCode,
                            Parcial = d.ParcialSol
                        }).ToList()
                };
            }

            // Correlativo del asiento: bloqueo por año, mes y tipo dentro de la transacción.
            await uow.AcquireAppLockAsync(
                AsientoNumeroGenerator.LockResource(anio, mes, plantilla.TipoAsientoCode), 15000, ct);

            var numero = await AsientoNumeroGenerator.NextAsync(uow, anio, mes, plantilla.TipoAsientoCode, ct);

            var asiento = new AsientoContable
            {
                Anio = anio,
                Mes = mes,
                TipoAsientoCode = plantilla.TipoAsientoCode,
                Numero = numero,
                Descripcion = glosa,
                FechaProceso = fechaRecepcion,
                TipoCambio = tipoCambioAsiento,
                Estado = EstadoAsiento.Pendiente,
                Origen = OrigenAsiento.Logistica
            };

            await AsientoPlantillaMotor.BuildAsync(uow, plantilla, fuente, asiento, ct);

            if (!esTransferencia)
            {
                await AsientoDestinoGenerator.ApplyAsync(uow, asiento, ct);
            }

            AsientoCuadre.EnsureBalanced(asiento);

            await uow.Contabilidad.Transacciones.AsientosContables.AddAsync(asiento, ct);

            if (documento is not null)
            {
                documento.AsientoProvisionCode = asiento.Codigo;
                await uow.Contabilidad.Transacciones.DocumentosCompra.AddAsync(documento, ct);
            }

            ni.AsientoContable = asiento.Codigo;

            await uow.SaveChangesAsync(ct);

            return new AsientoNotaIngresoResultResponse
            {
                PlantaCode = ni.PlantaCode,
                NotaIngresoCode = ni.Code,
                Success = true,
                AsientoCodigo = asiento.Codigo,
                DocumentoComprobantePagoCode = documento?.ComprobantePagoCode,
                DocumentoNumero = documento?.NumeroDocumento,
                Message = $"Asiento {asiento.Codigo} generado."
            };
        }

        private const string DocumentoCompraMonedaSoles = "PEN";

        /// <summary>
        /// Si el documento ya está registrado con su asiento de provisión, solo se vincula a la nota de ingreso
        /// (legacy). Si está registrado sin asiento, se rechaza (el legacy lo borraba y saltaba la nota).
        /// </summary>
        private static async Task<AsientoNotaIngresoResultResponse?> TryLinkExistingAsync(
            IUnitOfWork uow, Cepheus.Domain.Logistica.Transacciones.NotaIngreso ni, string etiqueta, CancellationToken ct)
        {
            var numero = (ni.NumeroDocumento ?? string.Empty).Trim();
            if (numero.Length == 0)
            {
                throw new InvalidOperationException($"La nota de ingreso {etiqueta} no tiene número de documento.");
            }

            var existente = await uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                .FirstOrDefaultAsync(d =>
                    d.ComprobantePagoCode == ni.ComprobantePagoCode
                    && d.NumeroDocumento == numero
                    && d.ProveedorCode == ni.ProveedorCode, ct);

            if (existente is null)
            {
                return null;
            }

            var asientoExistente = (existente.AsientoProvisionCode ?? string.Empty).Trim();
            if (asientoExistente.Length == AsientoContableRules.CodigoLength)
            {
                ni.AsientoContable = asientoExistente;
                await uow.SaveChangesAsync(ct);

                return new AsientoNotaIngresoResultResponse
                {
                    PlantaCode = ni.PlantaCode,
                    NotaIngresoCode = ni.Code,
                    Success = true,
                    AsientoCodigo = asientoExistente,
                    DocumentoComprobantePagoCode = existente.ComprobantePagoCode,
                    DocumentoNumero = existente.NumeroDocumento,
                    Vinculado = true,
                    Message = $"El documento ya estaba registrado con el asiento {asientoExistente}; se vinculó a la nota de ingreso."
                };
            }

            throw new InvalidOperationException(
                $"El documento {ni.ComprobantePagoCode} {numero} del proveedor {ni.ProveedorCode} ya está registrado en el " +
                "registro de compras sin asiento de provisión. Elimínelo antes de generar el asiento desde la nota de ingreso.");
        }

        /// <summary>
        /// Documento de compra desde la nota de ingreso (legacy: INSERT en RDocCompras / DDocCompras).
        /// Los importes se toman de la nota de ingreso (no se recalculan) y se expresan en soles; el total
        /// excluye el no gravable, como el legacy (la resta del no gravable está comentada en el programa).
        /// </summary>
        private static async Task<DocumentoCompra> BuildDocumentoAsync(
            IUnitOfWork uow,
            Cepheus.Domain.Logistica.Transacciones.NotaIngreso ni,
            string? cuentaCompras,
            string glosa,
            bool esSoles,
            decimal tca,
            CancellationToken ct)
        {
            var etiqueta = $"{ni.PlantaCode}-{ni.Code}";

            if (string.IsNullOrWhiteSpace(cuentaCompras))
            {
                throw new InvalidOperationException(
                    "La plantilla de provisión no tiene cuenta de compras para las líneas del documento.");
            }

            if (ni.FormaPago is null)
            {
                throw new InvalidOperationException(
                    $"No se registra Forma de Pago {ni.FormaPagoCode} {etiqueta}.");
            }

            if (ni.Detalles.Count == 0)
            {
                throw new InvalidOperationException($"No existe Detalle en la nota de ingreso {etiqueta}.");
            }

            var numero = ni.NumeroDocumento!.Trim();

            decimal S(decimal x) => esSoles ? x : Math.Round(x * tca, 2, MidpointRounding.AwayFromZero);

            var sbru = S(ni.Total);
            var sigv = S(ni.Igv);
            var sngr = S(ni.NoGravable);
            var sren = S(ni.Renta);
            var sfon = S(ni.Fonavi);
            var sser = S(ni.Servicio);
            var siex = S(ni.IgvExterior);
            var stot = sbru + sigv - sren - sfon + sser + siex;

            var esNotaCredito = ni.ComprobantePagoCode == AsientoNotaIngresoRules.NotaCreditoComprobanteCode;
            string? origenComprobante = null;
            string? origenNumero = null;

            if (esNotaCredito)
            {
                origenComprobante = string.IsNullOrWhiteSpace(ni.ComprobantePagoReferenciaCode)
                    ? "01"
                    : ni.ComprobantePagoReferenciaCode.Trim();
                origenNumero = (ni.NumeroReferencia ?? string.Empty).Trim();

                if (origenNumero.Length == 0)
                {
                    throw new InvalidOperationException(
                        $"La nota de crédito de la nota de ingreso {etiqueta} no indica el documento que referencia.");
                }

                var existeOrigen = await uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                    .AnyAsync(d => d.ComprobantePagoCode == origenComprobante
                                   && d.NumeroDocumento == origenNumero
                                   && d.ProveedorCode == ni.ProveedorCode, ct);

                if (!existeOrigen)
                {
                    throw new InvalidOperationException(
                        $"La nota de crédito {numero} referencia el documento {origenComprobante} {origenNumero}, " +
                        $"que no existe en el registro de compras del proveedor {ni.ProveedorCode}.");
                }
            }

            var fechaEmision = ni.FechaEmision.Date;

            var documento = new DocumentoCompra
            {
                ComprobantePagoCode = ni.ComprobantePagoCode,
                NumeroDocumento = numero,
                ProveedorCode = ni.ProveedorCode,
                FechaEmision = fechaEmision,
                FechaRecepcion = ni.FechaRecepcion.Date,
                FechaDocumento = ni.FechaDocumento,
                FechaVencimiento = fechaEmision.AddDays(ni.FormaPago.Days),
                FormaPagoCode = ni.FormaPagoCode,
                MonedaCode = ni.MonedaCode,
                TipoCambioFecha = esSoles ? null : DateOnly.FromDateTime(fechaEmision),
                ParcialSol = sbru,
                IgvSol = sigv,
                NoGravableSol = sngr,
                RentaSol = sren,
                FonaviSol = sfon,
                ServicioSol = sser,
                IgvExteriorSol = siex,
                TotalSol = stot,
                Glosa = AsientoNotaIngresoRules.Truncate(glosa, 50),
                Origen = OrigenDocumentoCompra.Logistica,
                Estado = EstadoDocumentoCompra.Pendiente,
                DocumentoOrigenComprobantePagoCode = origenComprobante,
                DocumentoOrigenNumero = origenNumero,
                PagadoSol = 0
            };

            var lineas = ni.Detalles.OrderBy(d => d.ItemNumber).ToList();
            var acumulado = 0m;
            var item = 0;

            for (var i = 0; i < lineas.Count; i++)
            {
                var linea = lineas[i];
                var spar = S(linea.Total);

                // En moneda extranjera la última línea absorbe el redondeo para que el detalle sume el parcial.
                if (!esSoles)
                {
                    if (i == lineas.Count - 1)
                    {
                        spar = sbru - acumulado;
                    }
                    else
                    {
                        acumulado += spar;
                    }
                }

                item++;
                documento.Detalles.Add(new DocumentoCompraDetalle
                {
                    ComprobantePagoCode = documento.ComprobantePagoCode,
                    NumeroDocumento = documento.NumeroDocumento,
                    ProveedorCode = documento.ProveedorCode,
                    ItemNumber = item,
                    PlantaCode = ni.PlantaCode,
                    CuentaContableCode = cuentaCompras.Trim().ToUpperInvariant(),
                    Detalle = AsientoNotaIngresoRules.Truncate(glosa, 50),
                    Cantidad = linea.Cantidad,
                    UnidadMedidaCode = linea.Articulo.UnidadMedidaCode,
                    PrecioSol = linea.Cantidad == 0 ? 0m : Math.Round(spar / linea.Cantidad, 2, MidpointRounding.AwayFromZero),
                    ParcialSol = spar
                });
            }

            return documento;
        }
    }
}
