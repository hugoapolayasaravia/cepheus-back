using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ComprobanteRetencionPagoProveedor
{
    public class ComprobanteRetencionPagoProveedorQueryHandler
        : IRequestHandler<ComprobanteRetencionPagoProveedorQuery, ComprobanteRetencionPagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public ComprobanteRetencionPagoProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ComprobanteRetencionPagoProveedorResponse> Handle(
            ComprobanteRetencionPagoProveedorQuery request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var pago = await _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .AsNoTracking()
                .Include(p => p.Proveedor)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.DocumentoCompra)
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken)
                ?? throw new KeyNotFoundException($"Pago {code} no encontrado.");

            var nombre = pago.Proveedor?.LegalName ?? string.Empty;

            return new ComprobanteRetencionPagoProveedorResponse
            {
                PagoCode = pago.Code,
                ProveedorCode = pago.ProveedorCode,
                ProveedorName = nombre.Length > 100 ? nombre[..100] : nombre,
                ProveedorRuc = pago.Proveedor?.DocumentNumber ?? string.Empty,
                NumeroComprobanteRetencion = pago.NumeroComprobanteRetencion,
                FechaComprobanteRetencion = pago.FechaComprobanteRetencion,
                Documentos = pago.Detalles
                    .OrderBy(d => d.Id)
                    .Select(d =>
                    {
                        var numero = d.NumeroDocumento.Trim();
                        var guion = numero.IndexOf('-');
                        return new ComprobanteRetencionItemResponse
                        {
                            ComprobantePagoCode = d.ComprobantePagoCode,
                            NumeroDocumento = d.NumeroDocumento,
                            Serie = guion >= 0 ? numero[..guion].TrimEnd() : numero,
                            Correlativo = guion >= 0 ? numero[(guion + 1)..].TrimEnd() : string.Empty,
                            FechaEmisionDocumento = d.DocumentoCompra?.FechaEmision ?? default,
                            BaseRetencionSol = d.BaseRetencionSol,
                            MontoRetenidoSol = d.MontoRetenidoSol
                        };
                    })
                    .ToList()
            };
        }
    }
}
