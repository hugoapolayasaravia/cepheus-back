using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common
{
    public static class PagoProveedorRecalculator
    {
        public static string Key(string comprobantePagoCode, string numeroDocumento, string proveedorCode)
            => $"{comprobantePagoCode}|{numeroDocumento}|{proveedorCode}";

        public static string Key(PagoProveedorDetalle l)
            => Key(l.ComprobantePagoCode, l.NumeroDocumento, l.ProveedorCode);

        /// <summary>Legacy ufn_valida_retencion_igv: factura (01) totalmente no gravable y sin IGV no se retiene.</summary>
        public static bool ValidaRetencionIgv(DocumentoCompra doc)
            => !(doc.ComprobantePagoCode == "01" && doc.ParcialSol == doc.NoGravableSol && doc.IgvSol == 0);

        /// <summary>
        /// Legacy ue_actualizache (solo soles): totales del pago, retención del IGV y su reparto
        /// por documento. Se retiene solo si el pago supera el tope (TControles.tope_retFac), el
        /// proveedor es afecto, hay factura/nota de débito/nota de crédito, no hay detracción y
        /// todos los documentos pasan ufn_valida_retencion_igv.
        /// </summary>
        public static async Task ApplyRetentionAsync(
            IUnitOfWork uow,
            PagoProveedor pago,
            IReadOnlyList<PagoProveedorDetalle> lines,
            IReadOnlyDictionary<string, DocumentoCompra> docs,
            CancellationToken cancellationToken)
        {
            var control = await uow.Comunes.ControlesVentas.Query().AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            if (control is null)
            {
                throw new InvalidOperationException(
                    "No hay controles configurados (porcentaje y tope de retención).");
            }

            var pagoSol = lines.Sum(l => l.PagoSol);
            var hayDocumentos = lines.Any(l => PagoProveedorRules.EsComprobanteDeRetencion(l.ComprobantePagoCode));

            decimal retencion = 0;
            if (pagoSol > control.WithholdingCapInvoice && pago.Afecto && hayDocumentos && !pago.Detraccion)
            {
                var todosValidos = lines.All(l => ValidaRetencionIgv(docs[Key(l)]));
                if (todosValidos)
                {
                    retencion = Math.Round(
                        pagoSol * control.WithholdingPercentage / 100m, 2, MidpointRounding.AwayFromZero);
                }
            }

            pago.PagoSol = pagoSol;
            pago.RetencionSol = retencion;
            pago.TotalSol = pagoSol - retencion;

            // Reparto por documento: proporcional al pago; el último absorbe la diferencia por redondeo.
            decimal acumuladoBase = 0;
            decimal acumuladoRetenido = 0;
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                line.BaseRetencionSol = 0;
                line.MontoRetenidoSol = 0;

                if (retencion <= 0)
                {
                    continue;
                }

                var baseSol = line.PagoSol;
                var retenido = Math.Round(baseSol * control.WithholdingPercentage / 100m, 2, MidpointRounding.AwayFromZero);

                if (i == lines.Count - 1)
                {
                    baseSol = pagoSol - acumuladoBase;
                    retenido = retencion - acumuladoRetenido;
                }
                else
                {
                    acumuladoBase += baseSol;
                    acumuladoRetenido += retenido;
                }

                line.BaseRetencionSol = baseSol;
                line.MontoRetenidoSol = retenido;
            }
        }

        /// <summary>
        /// Legacy co_actualiza_doc_compras: actualiza lo pagado del documento y su estado.
        /// Se aplica la variación (nuevo - anterior) para conservar otros pagos del documento
        /// (por ejemplo la detracción). En notas de crédito el pago va en negativo, por eso se
        /// usa el valor absoluto.
        /// </summary>
        public static void ApplyDocumentBalance(DocumentoCompra doc, decimal anteriorAbs, decimal nuevoAbs, DateTime fechaPago)
        {
            var pagado = doc.PagadoSol - anteriorAbs + nuevoAbs;
            doc.PagadoSol = pagado < 0 ? 0 : pagado;

            if (doc.TotalSol - doc.PagadoSol <= 0)
            {
                doc.Estado = EstadoDocumentoCompra.Cancelado;
                doc.FechaCancelacion = fechaPago.Date;
            }
            else
            {
                doc.Estado = EstadoDocumentoCompra.Pendiente;
                doc.FechaCancelacion = null;
            }
        }
    }
}
