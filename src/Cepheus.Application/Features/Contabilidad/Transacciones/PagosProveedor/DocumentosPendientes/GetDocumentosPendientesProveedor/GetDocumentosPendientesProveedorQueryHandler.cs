using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.DocumentosPendientes.GetDocumentosPendientesProveedor
{
    /// <summary>
    /// Legacy com_con_documentos: documentos del proveedor en estado Pendiente, en soles, con asiento
    /// de provisión. Se omiten los que no tienen saldo pendiente (legacy: pendiente &lt;= 0 se salta).
    /// </summary>
    public class GetDocumentosPendientesProveedorQueryHandler
        : IRequestHandler<GetDocumentosPendientesProveedorQuery, List<DocumentoPendientePagoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetDocumentosPendientesProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<DocumentoPendientePagoResponse>> Handle(
            GetDocumentosPendientesProveedorQuery request, CancellationToken cancellationToken)
        {
            var prv = PagoProveedorRules.Normalize(request.ProveedorCode);

            var docs = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                .AsNoTracking()
                .Where(d => d.ProveedorCode == prv
                            && d.Estado == EstadoDocumentoCompra.Pendiente
                            && d.MonedaCode == PagoProveedorRules.MonedaBase
                            && d.AsientoProvisionCode != null
                            && d.AsientoProvisionCode != string.Empty
                            && d.PendienteSol > 0)
                .OrderBy(d => d.ComprobantePagoCode)
                .ThenBy(d => d.NumeroDocumento)
                .ToListAsync(cancellationToken);

            return docs.Select(d =>
            {
                var sign = PagoProveedorRules.Sign(d.ComprobantePagoCode);
                var detraccion = PagoProveedorRules.TieneDetraccion(d);
                var montoDetraccion = detraccion ? d.MontoDetraccionSol : 0m;

                var pendiente = sign * d.PendienteSol;
                var sugerido = pendiente;
                if (d.PendienteSol == d.TotalSol)
                {
                    sugerido -= montoDetraccion;
                }

                return new DocumentoPendientePagoResponse
                {
                    ComprobantePagoCode = d.ComprobantePagoCode,
                    NumeroDocumento = d.NumeroDocumento,
                    ProveedorCode = d.ProveedorCode,
                    FechaEmision = d.FechaEmision,
                    FechaVencimiento = d.FechaVencimiento,
                    TotalSol = sign * d.TotalSol,
                    PendienteSol = pendiente,
                    PagoSugeridoSol = sugerido,
                    AsientoProvisionCode = d.AsientoProvisionCode,
                    Detraccion = detraccion,
                    MontoDetraccionSol = montoDetraccion
                };
            }).ToList();
        }
    }
}
