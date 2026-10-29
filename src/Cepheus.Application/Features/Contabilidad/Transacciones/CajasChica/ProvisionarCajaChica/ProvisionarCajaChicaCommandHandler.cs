using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.ProvisionarCajaChica
{
    public class ProvisionarCajaChicaCommandHandler : IRequestHandler<ProvisionarCajaChicaCommand, CajaChicaResponse>
    {
        /// <summary>Forma de pago y unidad de medida con las que el legacy generaba los documentos de caja chica.</summary>
        private const string FormaPagoCajaChica = "01";
        private const string UnidadMedidaCajaChica = "NIN";

        private readonly IUnitOfWork _uow;

        public ProvisionarCajaChicaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CajaChicaResponse> Handle(ProvisionarCajaChicaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var caja = await _uow.Contabilidad.Transacciones.CajasChica.Query()
                    .Include(c => c.Details)
                    .FirstOrDefaultAsync(c => c.Code == code, ct);

                if (caja is null)
                {
                    throw new KeyNotFoundException($"Caja chica {code} no encontrada.");
                }

                if (caja.Status != EstadoCajaChica.Pendiente)
                {
                    throw new InvalidOperationException($"La caja chica está {caja.Status}: solo se provisiona una caja Pendiente.");
                }

                if (!string.IsNullOrWhiteSpace(caja.ProvisionAsientoCode))
                {
                    throw new InvalidOperationException($"La caja chica ya tiene el asiento de provisión {caja.ProvisionAsientoCode.Trim()}.");
                }

                if (caja.Details.Count == 0)
                {
                    throw new InvalidOperationException("La caja chica no tiene documentos.");
                }

                var fecha = caja.IssueDate.Date;
                var anio = fecha.Year.ToString("0000");
                var mes = fecha.Month.ToString("00");

                await PeriodoContableRules.EnsureOpenAsync(_uow, anio, mes, ct);

                var tipoCambioAsiento = await TipoCambioRules.GetVentaAsync(_uow, TipoCambioRules.MonedaReferencia, fecha, ct);

                if (!tipoCambioAsiento.HasValue || tipoCambioAsiento.Value <= 0)
                {
                    throw new InvalidOperationException(
                        $"Tipo de cambio de la caja chica {code} no registrado para el {fecha:dd/MM/yyyy}.");
                }

                var plantilla = await CajaChicaAsientoService.GetPlantillaAsync(_uow, ProcesoAsiento.ProvisionCajaChica, ct);

                var proveedores = await CajaChicaAsientoService.LoadProveedoresAsync(_uow, caja.Details, ct);

                var conDocumento = caja.Details
                    .Where(d => d.ComprobantePagoCode != CajaChicaRules.ComprobanteSinDocumento)
                    .OrderBy(d => d.ItemNumber)
                    .ToList();

                // Documentos: se reemplazan los que una provisión anterior de caja chica dejó sin asiento
                // (como el SP co_exporta_cajachica_doc_compras); cualquier otro registro existente bloquea.
                var formaPago = await _uow.Logistica.Catalogos.FormasPago.Query().AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Code == FormaPagoCajaChica, ct)
                    ?? throw new InvalidOperationException($"No existe la forma de pago {FormaPagoCajaChica} usada por los documentos de caja chica.");

                var unidadOk = await _uow.Logistica.Catalogos.UnidadesMedida.Query()
                    .AnyAsync(u => u.Code == UnidadMedidaCajaChica, ct);

                if (conDocumento.Count > 0 && !unidadOk)
                {
                    throw new InvalidOperationException($"No existe la unidad de medida {UnidadMedidaCajaChica} usada por los documentos de caja chica.");
                }

                var eliminados = false;
                foreach (var d in conDocumento)
                {
                    var existente = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                        .Include(x => x.Detalles)
                        .FirstOrDefaultAsync(x => x.ComprobantePagoCode == d.ComprobantePagoCode
                                                  && x.NumeroDocumento == d.NumeroDocumento
                                                  && x.ProveedorCode == d.ProveedorCode, ct);

                    if (existente is null)
                    {
                        continue;
                    }

                    if (existente.Origen != OrigenDocumentoCompra.CajaChica
                        || !string.IsNullOrWhiteSpace(existente.AsientoProvisionCode))
                    {
                        throw new InvalidOperationException(
                            $"El documento {d.ComprobantePagoCode} {d.NumeroDocumento} del proveedor {d.ProveedorCode} ya está en el registro de compras.");
                    }

                    foreach (var det in existente.Detalles.ToList())
                    {
                        _uow.Contabilidad.Transacciones.DocumentoCompraDetalles.Remove(det);
                    }

                    _uow.Contabilidad.Transacciones.DocumentosCompra.Remove(existente);
                    eliminados = true;
                }

                if (eliminados)
                {
                    await _uow.SaveChangesAsync(ct);
                }

                // Asiento por plantilla: un bloque de líneas por documento, todos en un mismo asiento.
                var fuentes = await CajaChicaAsientoService.BuildFuentesAsync(_uow, caja, plantilla, proveedores, ct);

                var asiento = await CajaChicaAsientoService.BuildAsientoAsync(
                    _uow, plantilla, fuentes, anio, mes, fecha, $"CAJA CHICA {code}",
                    tipoCambioAsiento.Value, aplicarDestinos: true, ct);

                // Documentos de compra (origen caja chica)
                foreach (var d in conDocumento)
                {
                    var documento = await BuildDocumentoAsync(caja, d, formaPago.Days, asiento.Codigo, conDocumento, ct);
                    await _uow.Contabilidad.Transacciones.DocumentosCompra.AddAsync(documento, ct);
                }

                // Notas de ingreso / órdenes de servicio incluidas quedan marcadas con el asiento
                foreach (var d in caja.Details.Where(x => x.Origen != OrigenLineaCajaChica.Manual))
                {
                    var referencia = d.ReferenceCode!;
                    var planta = d.PlantaCode;

                    if (d.Origen == OrigenLineaCajaChica.NotaIngreso)
                    {
                        var ni = await _uow.Logistica.Transacciones.NotaIngresos.Query()
                            .FirstOrDefaultAsync(n => n.PlantaCode == planta && n.Code == referencia, ct)
                            ?? throw new InvalidOperationException($"Error al actualizar la nota de ingreso {planta}-{referencia}.");

                        if (!string.IsNullOrWhiteSpace(ni.AsientoContable))
                        {
                            throw new InvalidOperationException(
                                $"La nota de ingreso {planta}-{referencia} ya tiene el asiento {ni.AsientoContable.Trim()}.");
                        }

                        ni.AsientoContable = asiento.Codigo;
                    }
                    else
                    {
                        var os = await _uow.Logistica.Transacciones.OrdenesServicio.Query()
                            .FirstOrDefaultAsync(n => n.PlantaCode == planta && n.Code == referencia, ct)
                            ?? throw new InvalidOperationException($"Error al actualizar la orden de servicio {planta}-{referencia}.");

                        if (!string.IsNullOrWhiteSpace(os.AsientoContable))
                        {
                            throw new InvalidOperationException(
                                $"La orden de servicio {planta}-{referencia} ya tiene el asiento {os.AsientoContable.Trim()}.");
                        }

                        os.AsientoContable = asiento.Codigo;
                    }
                }

                caja.ProvisionAsientoCode = asiento.Codigo;
                caja.Status = EstadoCajaChica.Procesado;

                await _uow.SaveChangesAsync(ct);

                return true;
            }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);

            _uow.ClearTracking();

            return (await CajaChicaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }

        private async Task<DocumentoCompra> BuildDocumentoAsync(
            CajaChica caja, CajaChicaDetalle d, int diasFormaPago, string asientoCodigo,
            IReadOnlyList<CajaChicaDetalle> documentosDeLaCaja, CancellationToken ct)
        {
            var s = CajaChicaAsientoService.ToSoles(d);
            var esBase = TipoCambioRules.EsMonedaBase(d.MonedaCode);
            var esNotaCredito = d.ComprobantePagoCode == AsientoNotaIngresoRules.NotaCreditoComprobanteCode;

            string? origenComprobante = null;
            string? origenNumero = null;

            if (esNotaCredito)
            {
                origenComprobante = d.DocumentoReferenciaComprobantePagoCode;
                origenNumero = d.DocumentoReferenciaNumero;

                if (string.IsNullOrWhiteSpace(origenComprobante) || string.IsNullOrWhiteSpace(origenNumero))
                {
                    throw new InvalidOperationException(
                        $"La nota de crédito {d.NumeroDocumento} de la caja no indica el documento que referencia.");
                }

                var enLaCaja = documentosDeLaCaja.Any(x => x.ComprobantePagoCode == origenComprobante
                                                          && x.NumeroDocumento == origenNumero
                                                          && x.ProveedorCode == d.ProveedorCode);

                var enCompras = enLaCaja || await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                    .AnyAsync(x => x.ComprobantePagoCode == origenComprobante
                                   && x.NumeroDocumento == origenNumero
                                   && x.ProveedorCode == d.ProveedorCode, ct);

                if (!enCompras)
                {
                    throw new InvalidOperationException(
                        $"La nota de crédito {d.NumeroDocumento} referencia el documento {origenComprobante} {origenNumero}, " +
                        $"que no existe en el registro de compras del proveedor {d.ProveedorCode}.");
                }
            }

            var fechaEmision = d.FechaEmision.Date;
            var glosa = AsientoNotaIngresoRules.Truncate($"Generación desde caja chica número : {caja.Code}", 50);
            var detalle = AsientoNotaIngresoRules.Truncate(
                $"Caja Chica : {caja.Code} {(d.Concepto ?? string.Empty).Trim()}".Trim(), 50);

            var documento = new DocumentoCompra
            {
                ComprobantePagoCode = d.ComprobantePagoCode,
                NumeroDocumento = d.NumeroDocumento,
                ProveedorCode = d.ProveedorCode,
                FechaEmision = fechaEmision,
                FechaRecepcion = caja.IssueDate.Date,
                FechaVencimiento = fechaEmision.AddDays(diasFormaPago),
                FormaPagoCode = FormaPagoCajaChica,
                MonedaCode = d.MonedaCode,
                TipoCambioFecha = esBase ? null : DateOnly.FromDateTime(fechaEmision),
                ParcialSol = s.Parcial,
                IgvSol = s.Igv,
                NoGravableSol = s.NoGravable,
                RentaSol = s.Renta,
                FonaviSol = s.Fonavi,
                ServicioSol = s.Servicio,
                IgvExteriorSol = s.IgvExterior,
                TotalSol = s.Total,
                Glosa = glosa,
                Origen = OrigenDocumentoCompra.CajaChica,
                Estado = EstadoDocumentoCompra.Pendiente,
                DocumentoOrigenComprobantePagoCode = origenComprobante,
                DocumentoOrigenNumero = origenNumero,
                AsientoProvisionCode = asientoCodigo,
                PagadoSol = 0
            };

            documento.Detalles.Add(new DocumentoCompraDetalle
            {
                ComprobantePagoCode = d.ComprobantePagoCode,
                NumeroDocumento = d.NumeroDocumento,
                ProveedorCode = d.ProveedorCode,
                ItemNumber = 1,
                PlantaCode = d.PlantaCode,
                CuentaContableCode = d.CuentaContableCode,
                AnexoTipoCode = d.AnexoTipoCode,
                AnexoCode = d.AnexoCode,
                Detalle = detalle,
                Cantidad = d.Cantidad,
                UnidadMedidaCode = UnidadMedidaCajaChica,
                PrecioSol = d.Cantidad == 0 ? 0m : Math.Round(s.Parcial / d.Cantidad, 2, MidpointRounding.AwayFromZero),
                ParcialSol = s.Parcial
            });

            return documento;
        }
    }
}
