using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.MarcarImpresionPagoProveedor
{
    public class MarcarImpresionPagoProveedorCommandHandler
        : IRequestHandler<MarcarImpresionPagoProveedorCommand, PagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public MarcarImpresionPagoProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagoProveedorResponse> Handle(
            MarcarImpresionPagoProveedorCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var pago = await _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken)
                ?? throw new KeyNotFoundException("Codigo de Operacion no existe Verifique por favor ..");

            if ((pago.AsientoCancelacionCode ?? string.Empty).Trim().Length != PagoProveedorRules.AsientoLength)
            {
                throw new InvalidOperationException("Asiento Contable no Generado...");
            }

            if (string.Equals(request.Tipo.Trim(), "Retencion", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(pago.NumeroComprobanteRetencion))
                {
                    throw new InvalidOperationException("Operacion no cuenta con Comprobante de Retencion ...");
                }

                if (!pago.ComprobanteCajaBancosImpreso)
                {
                    throw new InvalidOperationException("Comprobante Caja Bancos no Impreso ...");
                }

                pago.ComprobanteRetencionImpreso = true;
            }
            else
            {
                pago.ComprobanteCajaBancosImpreso = true;
            }

            await _uow.SaveChangesAsync(cancellationToken);
            _uow.ClearTracking();

            return (await PagoProveedorMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
