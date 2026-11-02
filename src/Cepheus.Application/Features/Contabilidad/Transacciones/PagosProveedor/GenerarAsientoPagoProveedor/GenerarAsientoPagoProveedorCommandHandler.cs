using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GenerarAsientoPagoProveedor
{
    /// <summary>
    /// Legacy ue_set_btn_generar + fc_genasicanprv + fc_generadestino. Todo en una transacción:
    /// valida el pago, toma el número del comprobante de retención (si corresponde), arma el asiento con la
    /// plantilla y deja el código del asiento en el pago.
    /// </summary>
    public class GenerarAsientoPagoProveedorCommandHandler
        : IRequestHandler<GenerarAsientoPagoProveedorCommand, PagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GenerarAsientoPagoProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagoProveedorResponse> Handle(
            GenerarAsientoPagoProveedorCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var pago = await _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                        .Include(p => p.Detalles)
                        .FirstOrDefaultAsync(p => p.Code == code, ct);

                    if (pago is null)
                    {
                        throw new KeyNotFoundException($"Pago {code} no encontrado.");
                    }

                    if (pago.Estado != EstadoPagoProveedor.Pendiente)
                    {
                        throw new InvalidOperationException("Asiento no puede ser generado: el pago no está Pendiente.");
                    }

                    if (!string.IsNullOrWhiteSpace(pago.AsientoCancelacionCode))
                    {
                        throw new InvalidOperationException(
                            $"El asiento de cancelación {pago.AsientoCancelacionCode.Trim()} ya fue generado.");
                    }

                    if (string.IsNullOrWhiteSpace(pago.NumeroCheque)
                        && !pago.Canje
                        && pago.NumeroControl != PagoProveedorRules.SinCheque)
                    {
                        throw new InvalidOperationException("Nro. de Cheque no registrado ...");
                    }

                    if (pago.Detalles.Count == 0)
                    {
                        throw new InvalidOperationException($"Cancelación {code} no registra documentos ... Reintente ...");
                    }

                    if (pago.TotalSol <= 0)
                    {
                        throw new InvalidOperationException("El total a pagar debe ser mayor a cero.");
                    }

                    var sinProvision = pago.Detalles.FirstOrDefault(
                        d => string.IsNullOrWhiteSpace(d.AsientoProvisionCode)
                             || d.AsientoProvisionCode.Trim().Length != PagoProveedorRules.AsientoLength);
                    if (sinProvision is not null)
                    {
                        throw new InvalidOperationException(
                            $"Documento {sinProvision.ComprobantePagoCode} {sinProvision.NumeroDocumento} no registra Asiento de provision ... Reintente ...");
                    }

                    // Legacy: gs_cierre >= año y mes del documento -> período cerrado.
                    await PagoProveedorRules.EnsurePeriodOpenAsync(_uow, pago.FechaEmision, ct);

                    var proveedor = await _uow.Logistica.Maestros.Proveedores.Query()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Code == pago.ProveedorCode, ct)
                        ?? throw new KeyNotFoundException($"Proveedor {pago.ProveedorCode} no encontrado.");

                    var plantilla = await AsientoCancelacionProveedorGenerator.GetPlantillaAsync(_uow, ct);

                    // Comprobante de retención: solo si hay retención y no hay detracción; el pago pasa a Procesado.
                    if (pago.RetencionSol > 0 && !pago.Detraccion)
                    {
                        pago.NumeroComprobanteRetencion =
                            await ComprobanteRetencionNumerador.TomarAsync(_uow, pago.PlantaCode, ct);
                        pago.FechaComprobanteRetencion = pago.FechaEmision.Date;
                        pago.Estado = EstadoPagoProveedor.Procesado;
                    }

                    var asiento = await AsientoCancelacionProveedorGenerator.BuildAsync(
                        _uow, pago, proveedor, plantilla, ct);

                    pago.AsientoCancelacionCode = asiento.Codigo;

                    await _uow.SaveChangesAsync(ct);
                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El pago fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();
            return (await PagoProveedorMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
