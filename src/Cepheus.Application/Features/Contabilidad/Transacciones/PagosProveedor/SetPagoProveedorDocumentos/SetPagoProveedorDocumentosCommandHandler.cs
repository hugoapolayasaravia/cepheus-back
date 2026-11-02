using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.SetPagoProveedorDocumentos
{
    /// <summary>
    /// Legacy ue_carga_detalle + ue_control + ue_set_btn_grb_data (paso 2) + ue_actualizache +
    /// actualiza_doc: define los documentos del pago, calcula el pago por documento, la retención y
    /// actualiza lo pagado y el estado de cada documento de compra, todo en una sola transacción.
    /// </summary>
    public class SetPagoProveedorDocumentosCommandHandler
        : IRequestHandler<SetPagoProveedorDocumentosCommand, PagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public SetPagoProveedorDocumentosCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagoProveedorResponse> Handle(
            SetPagoProveedorDocumentosCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var pago = await _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            if (pago is null)
            {
                throw new KeyNotFoundException($"Pago {code} no encontrado.");
            }

            PagoProveedorRules.EnsureEditable(pago);
            await PagoProveedorRules.EnsurePeriodOpenAsync(_uow, pago.FechaEmision, cancellationToken);

            var oldLines = pago.Detalles.ToList();
            var oldByKey = oldLines.ToDictionary(l => PagoProveedorRecalculator.Key(l));
            var oldAbsByKey = oldLines
                .GroupBy(l => PagoProveedorRecalculator.Key(l))
                .ToDictionary(g => g.Key, g => g.Sum(l => Math.Abs(l.PagoSol)));

            // Documentos afectados (nuevos y anteriores), con seguimiento para actualizar su saldo.
            var requested = request.Documentos
                .Select(d => (Tdo: PagoProveedorRules.Normalize(d.ComprobantePagoCode), Num: d.NumeroDocumento.Trim(), Input: d))
                .ToList();

            var docs = new Dictionary<string, DocumentoCompra>();
            var keysToLoad = requested.Select(r => (r.Tdo, r.Num))
                .Concat(oldLines.Select(l => (l.ComprobantePagoCode, l.NumeroDocumento)))
                .Distinct()
                .ToList();

            foreach (var (tdo, num) in keysToLoad)
            {
                var doc = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                    .FirstOrDefaultAsync(d => d.ComprobantePagoCode == tdo
                                              && d.NumeroDocumento == num
                                              && d.ProveedorCode == pago.ProveedorCode,
                        cancellationToken);

                if (doc is null)
                {
                    throw new KeyNotFoundException(
                        $"Documento {tdo} {num} del proveedor {pago.ProveedorCode} no encontrado.");
                }

                docs[PagoProveedorRecalculator.Key(tdo, num, pago.ProveedorCode)] = doc;
            }

            // Línea por documento.
            var finalLines = new List<PagoProveedorDetalle>();
            bool? primeraDetraccion = null;

            foreach (var (tdo, num, input) in requested)
            {
                var key = PagoProveedorRecalculator.Key(tdo, num, pago.ProveedorCode);
                var doc = docs[key];

                if (doc.MonedaCode != PagoProveedorRules.MonedaBase)
                {
                    throw new InvalidOperationException($"El documento {tdo} {num} no está en soles.");
                }

                if (string.IsNullOrWhiteSpace(doc.AsientoProvisionCode)
                    || doc.AsientoProvisionCode.Trim().Length != PagoProveedorRules.AsientoLength)
                {
                    throw new InvalidOperationException(
                        $"El documento {tdo} {num} no registra asiento de provisión.");
                }

                var anteriorAbs = oldAbsByKey.GetValueOrDefault(key);

                // Disponible: pendiente actual más lo que este mismo pago ya tenía aplicado.
                var disponible = doc.PendienteSol + anteriorAbs;
                if (disponible <= 0)
                {
                    throw new InvalidOperationException($"El documento {tdo} {num} no tiene saldo pendiente.");
                }

                var sign = PagoProveedorRules.Sign(tdo);
                var detraccion = PagoProveedorRules.TieneDetraccion(doc);

                var sugerido = sign * disponible;
                if (disponible == doc.TotalSol && detraccion)
                {
                    sugerido -= doc.MontoDetraccionSol;
                }

                var pagoSol = input.PagoSol ?? sugerido;

                // Legacy: las filas con pago 0 se descartan.
                if (pagoSol == 0)
                {
                    continue;
                }

                var validoPositivo = sign > 0 && pagoSol > 0 && pagoSol <= disponible;
                var validoNegativo = sign < 0 && pagoSol < 0 && pagoSol >= -disponible;
                if (!validoPositivo && !validoNegativo)
                {
                    var rango = sign > 0 ? $"entre 0 y {disponible:0.00}" : $"entre -{disponible:0.00} y 0";
                    throw new InvalidOperationException(
                        $"El pago del documento {tdo} {num} debe estar {rango}.");
                }

                // Legacy: todos los documentos del pago comparten el estado de detracción del primero;
                // la nota de crédito se admite aunque el primer documento tenga detracción.
                if (primeraDetraccion is null)
                {
                    primeraDetraccion = detraccion;
                }
                else if (detraccion != primeraDetraccion && !(tdo == PagoProveedorRules.NotaCredito && primeraDetraccion == true))
                {
                    throw new InvalidOperationException(
                        $"El documento {tdo} {num} no puede ir en el mismo pago: " +
                        "todos los documentos deben tener el mismo estado de detracción.");
                }

                PagoProveedorDetalle line;
                if (oldByKey.TryGetValue(key, out var existing))
                {
                    line = existing;
                }
                else
                {
                    line = new PagoProveedorDetalle
                    {
                        PagoCode = pago.Code,
                        ComprobantePagoCode = tdo,
                        NumeroDocumento = num,
                        ProveedorCode = pago.ProveedorCode
                    };
                    pago.Detalles.Add(line);
                }

                line.TotalSol = sign * doc.TotalSol;
                line.PendienteSol = sign * disponible;
                line.PagoSol = pagoSol;
                line.AsientoProvisionCode = doc.AsientoProvisionCode.Trim();
                line.Detraccion = detraccion;

                finalLines.Add(line);
            }

            // Se eliminan las líneas anteriores que ya no están.
            foreach (var old in oldLines.Where(o => !finalLines.Contains(o)))
            {
                pago.Detalles.Remove(old);
                _uow.Contabilidad.Transacciones.PagoProveedorDetalles.Remove(old);
            }

            pago.Detraccion = finalLines.Count > 0 && finalLines[0].Detraccion;

            await PagoProveedorRecalculator.ApplyRetentionAsync(_uow, pago, finalLines, docs, cancellationToken);

            // Saldo y estado de cada documento afectado.
            var nuevoAbsByKey = finalLines
                .GroupBy(l => PagoProveedorRecalculator.Key(l))
                .ToDictionary(g => g.Key, g => g.Sum(l => Math.Abs(l.PagoSol)));

            foreach (var (key, doc) in docs)
            {
                PagoProveedorRecalculator.ApplyDocumentBalance(
                    doc,
                    oldAbsByKey.GetValueOrDefault(key),
                    nuevoAbsByKey.GetValueOrDefault(key),
                    pago.FechaEmision);
            }

            await _uow.SaveChangesAsync(cancellationToken);
            _uow.ClearTracking();

            return (await PagoProveedorMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
