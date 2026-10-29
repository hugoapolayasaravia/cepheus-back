using System.Data;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.Common
{
    /// <summary>
    /// Generación del asiento contable de una orden de servicio (legacy: evento ue_generaasientoind de
    /// w_con_asientosordenservicio + fc_genasiprocompras), en UNA transacción y guiada por la plantilla activa.
    ///
    /// Proceso ProvisionOrdenServicio:
    ///   1. Valida cada línea: la cuenta es la del subcentro de costo y, si exige tipo de anexo, el anexo sale de
    ///      ArticuloAnexo / SubFamiliaAnexo (AnexoEnlaceResolver).
    ///   2. Crea el documento de compra (origen Logística) con esas líneas. Si el documento ya está registrado, se
    ///      rechaza (en la orden de servicio el legacy no vincula).
    ///   3. Arma el asiento con la plantilla, genera los destinos y exige que cuadre.
    ///   4. Guarda el código del asiento en el documento, en la orden, en su salida y en las líneas de la salida.
    /// Proceso TransferenciaOrdenServicio (comprobante "00" y número que empieza con TRANSFERENCIA): asiento directo
    /// con la plantilla, sin documento ni destinos (réplica del evento ue_transferencia del legacy).
    /// </summary>
    public static class AsientoOrdenServicioGenerator
    {
        private const string MonedaSoles = "PEN";
        private const string ComprobanteTransferencia = "00";
        private const string PrefijoTransferencia = "TRANSFERENCIA";
        private const string ComprobanteBoleta = "03";

        public static Task<AsientoOrdenServicioResultResponse> GenerateAsync(
            IUnitOfWork uow, string plantaCode, string ordenServicioCode, CancellationToken cancellationToken)
        {
            var planta = plantaCode.Trim().ToUpperInvariant();
            var codigo = ordenServicioCode.Trim().ToUpperInvariant();

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

        private static async Task<AsientoOrdenServicioResultResponse> GenerateCoreAsync(
            IUnitOfWork uow, string plantaCode, string osCode, CancellationToken ct)
        {
            var os = await uow.Logistica.Transacciones.OrdenesServicio.Query()
                .Include(o => o.Proveedor)
                .Include(o => o.ComprobantePago)
                .Include(o => o.FormaPago)
                .Include(o => o.Detalles).ThenInclude(d => d.Articulo)
                .Include(o => o.Detalles).ThenInclude(d => d.SubCentroCosto)
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == osCode, ct)
                ?? throw new KeyNotFoundException($"Orden de servicio {plantaCode}-{osCode} no encontrada.");

            var etiqueta = $"{os.PlantaCode}-{os.Code}";

            if (os.Estado != EstadoOrdenServicio.Procesado)
            {
                throw new InvalidOperationException(
                    $"La orden de servicio {etiqueta} está en estado {os.Estado}; solo se genera asiento de órdenes de servicio Procesadas.");
            }

            if (!string.IsNullOrWhiteSpace(os.AsientoContable))
            {
                throw new InvalidOperationException(
                    $"La orden de servicio {etiqueta} ya tiene el asiento {os.AsientoContable.Trim()}.");
            }

            if (os.ComprobantePago is null || !os.ComprobantePago.IsActive)
            {
                throw new InvalidOperationException(
                    $"El comprobante de pago {os.ComprobantePagoCode} de la orden de servicio {etiqueta} no existe o está inactivo.");
            }

            var numeroDoc = (os.NumeroDocumento ?? string.Empty).Trim();
            var esTransferencia = os.ComprobantePagoCode == ComprobanteTransferencia
                                  && numeroDoc.StartsWith(PrefijoTransferencia, StringComparison.Ordinal);

            var proceso = esTransferencia
                ? ProcesoAsiento.TransferenciaOrdenServicio
                : ProcesoAsiento.ProvisionOrdenServicio;

            var plantilla = await uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .AsNoTracking()
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Process == proceso && p.IsActive, ct)
                ?? throw new InvalidOperationException(
                    $"No existe una plantilla de asiento activa para el proceso {proceso}.");

            var fechaRecepcion = os.FechaRecepcion.Date;
            var anio = fechaRecepcion.Year.ToString("0000");
            var mes = fechaRecepcion.Month.ToString("00");

            await PeriodoContableRules.EnsureOpenAsync(uow, anio, mes, ct);

            var glosa = $"{os.PlantaCode} Orden de Servicio {os.Code}";
            var esSoles = string.Equals(os.MonedaCode, MonedaSoles, StringComparison.OrdinalIgnoreCase);

            DocumentoCompra? documento = null;
            AsientoFuente fuente;
            decimal tipoCambioAsiento;

            if (esTransferencia)
            {
                var venta = await GetVentaAsync(uow, os.FechaEmision, ct) ?? 0m;
                if (venta <= 0)
                {
                    throw new InvalidOperationException($"Tipo de Cambio es cero {etiqueta} ... Verifique ...");
                }

                tipoCambioAsiento = venta;

                decimal ToSolesTransferencia(decimal x) =>
                    esSoles ? x : Math.Round(x * venta, 2, MidpointRounding.AwayFromZero);

                fuente = new AsientoFuente
                {
                    PlantaCode = os.PlantaCode,
                    Glosa = glosa,
                    NumeroDocumento = os.NumeroDocumento,
                    ComprobantePagoCode = os.ComprobantePagoCode,
                    EsNotaCredito = false,
                    MontoNotaIngreso = ToSolesTransferencia(os.Monto),
                    NetoNotaIngreso = ToSolesTransferencia(os.Total)
                };
            }
            else
            {
                if (numeroDoc.Length == 0)
                {
                    throw new InvalidOperationException($"La orden de servicio {etiqueta} no tiene número de documento.");
                }

                var existe = await uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                    .AnyAsync(d => d.ComprobantePagoCode == os.ComprobantePagoCode
                                   && d.NumeroDocumento == numeroDoc
                                   && d.ProveedorCode == os.ProveedorCode, ct);

                if (existe)
                {
                    throw new InvalidOperationException(
                        $"Documento {os.ComprobantePagoCode} {numeroDoc} {os.ProveedorCode} ya está registrado en el registro de compras ... Verifique ...");
                }

                var venta = await GetVentaAsync(uow, os.FechaEmision, ct);
                var tca = esSoles ? (venta ?? os.TipoCambio) : os.TipoCambio;
                if (tca <= 0)
                {
                    throw new InvalidOperationException($"Tipo de Cambio es cero {etiqueta} ... Verifique ...");
                }

                tipoCambioAsiento = tca;

                documento = await BuildDocumentoAsync(uow, os, glosa, numeroDoc, esSoles, tca, ct);

                var cuentaPorPagar = plantilla.Lines.Any(l => l.AccountSource == OrigenCuentaAsiento.CuentaPorPagarProveedor)
                    ? await ProveedorCuentaPorPagarResolver.ResolverAsync(uow, os.ProveedorCode, os.MonedaCode, ct)
                    : null;

                fuente = new AsientoFuente
                {
                    PlantaCode = os.PlantaCode,
                    Glosa = glosa,
                    NumeroDocumento = documento.NumeroDocumento,
                    ComprobantePagoCode = documento.ComprobantePagoCode,
                    EsNotaCredito = documento.ComprobantePagoCode == AsientoNotaIngresoRules.NotaCreditoComprobanteCode,
                    ProveedorCode = os.ProveedorCode,
                    ProveedorNombre = os.Proveedor?.LegalName,
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

            // Marca la orden de servicio, su salida y las líneas de la salida con el asiento.
            os.AsientoContable = asiento.Codigo;

            var salida = await uow.Logistica.Transacciones.OrdenesServicioSalida.Query()
                .Include(s => s.Detalles)
                .FirstOrDefaultAsync(s => s.PlantaCode == os.PlantaCode && s.Code == os.ValeSalidaCode, ct);

            if (salida is not null)
            {
                salida.AsientoContable = asiento.Codigo;
                foreach (var linea in salida.Detalles)
                {
                    linea.AsientoContable = asiento.Codigo;
                }
            }

            await uow.SaveChangesAsync(ct);

            return new AsientoOrdenServicioResultResponse
            {
                PlantaCode = os.PlantaCode,
                OrdenServicioCode = os.Code,
                Success = true,
                AsientoCodigo = asiento.Codigo,
                DocumentoComprobantePagoCode = documento?.ComprobantePagoCode,
                DocumentoNumero = documento?.NumeroDocumento,
                Message = $"Asiento {asiento.Codigo} generado."
            };
        }

        /// <summary>
        /// Documento de compra desde la orden de servicio (legacy: INSERT en RDocCompras / DDocCompras). Los importes se
        /// toman de la orden (no se recalculan) y se expresan en soles; el total excluye el no gravable, como el legacy.
        /// Con boleta (comprobante 03) el no gravable del documento es igual al parcial.
        /// </summary>
        private static async Task<DocumentoCompra> BuildDocumentoAsync(
            IUnitOfWork uow, OrdenServicio os, string glosa, string numero, bool esSoles, decimal tca, CancellationToken ct)
        {
            var etiqueta = $"{os.PlantaCode}-{os.Code}";

            if (os.FormaPago is null)
            {
                throw new InvalidOperationException($"No se registra Forma de Pago {os.FormaPagoCode} {etiqueta}.");
            }

            if (os.Detalles.Count == 0)
            {
                throw new InvalidOperationException($"No existe Detalle en la orden de servicio {etiqueta}.");
            }

            decimal S(decimal x) => esSoles ? x : Math.Round(x * tca, 2, MidpointRounding.AwayFromZero);

            var sbru = S(os.Total);
            var sigv = S(os.Igv);
            var sngr = os.ComprobantePagoCode == ComprobanteBoleta ? sbru : S(os.NoGravable);
            var sren = S(os.Renta);
            var sfon = S(os.Fonavi);
            var sser = S(os.Servicio);
            var siex = S(os.IgvExterior);
            var stot = sbru + sigv - sren - sfon + sser + siex;

            var fechaEmision = os.FechaEmision.Date;

            var documento = new DocumentoCompra
            {
                ComprobantePagoCode = os.ComprobantePagoCode,
                NumeroDocumento = numero,
                ProveedorCode = os.ProveedorCode,
                FechaEmision = fechaEmision,
                FechaRecepcion = os.FechaRecepcion.Date,
                FechaVencimiento = fechaEmision.AddDays(os.FormaPago.Days),
                FormaPagoCode = os.FormaPagoCode,
                MonedaCode = os.MonedaCode,
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
                PagadoSol = 0
            };

            var lineas = os.Detalles.OrderBy(d => d.ItemNumber).ToList();

            // Primero se validan todas las líneas (como el legacy), luego se arman.
            var resueltas = new List<AnexoEnlaceResolver.Resultado>();
            foreach (var linea in lineas)
            {
                resueltas.Add(await AnexoEnlaceResolver.ResolverAsync(
                    uow,
                    linea.SubCentroCostoCode,
                    linea.SubCentroCosto?.AccountingAccountCode,
                    linea.ArticuloCode,
                    ct));
            }

            var acumulado = 0m;
            var item = 0;

            for (var i = 0; i < lineas.Count; i++)
            {
                var linea = lineas[i];
                var spar = S(linea.Total);

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
                    PlantaCode = linea.PlantaAfectadaCode,
                    CuentaContableCode = resueltas[i].CuentaCode,
                    AnexoTipoCode = resueltas[i].AnexoTipoCode,
                    AnexoCode = resueltas[i].AnexoCode,
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
