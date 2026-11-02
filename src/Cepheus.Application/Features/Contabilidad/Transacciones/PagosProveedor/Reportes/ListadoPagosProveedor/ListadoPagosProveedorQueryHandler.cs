using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ListadoPagosProveedor
{
    public class ListadoPagosProveedorQueryHandler
        : IRequestHandler<ListadoPagosProveedorQuery, List<ListadoPagoProveedorRowResponse>>
    {
        private readonly IUnitOfWork _uow;

        public ListadoPagosProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ListadoPagoProveedorRowResponse>> Handle(
            ListadoPagosProveedorQuery request, CancellationToken cancellationToken)
        {
            IQueryable<PagoProveedor> query = _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .AsNoTracking()
                .Include(p => p.Proveedor)
                .Include(p => p.Banco)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.DocumentoCompra);

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                // Legacy: con número de operación no se aplican los demás filtros.
                var code = request.Code.Trim();
                query = query.Where(p => p.Code == code);
            }
            else
            {
                var desde = request.FechaDesde!.Value.Date;
                var hasta = request.FechaHasta!.Value.Date;
                query = query.Where(p => p.FechaEmision >= desde && p.FechaEmision <= hasta);

                if (!string.IsNullOrWhiteSpace(request.NumeroCheque))
                {
                    var cheque = request.NumeroCheque.Trim();
                    query = query.Where(p => p.NumeroCheque == cheque);
                }

                if (!string.IsNullOrWhiteSpace(request.NumeroComprobanteRetencion))
                {
                    var numero = request.NumeroComprobanteRetencion.Trim();
                    query = query.Where(p => p.NumeroComprobanteRetencion == numero);
                }

                if (!string.IsNullOrWhiteSpace(request.BancoCode))
                {
                    var banco = request.BancoCode.Trim().ToUpper();
                    query = query.Where(p => p.BancoCode == banco);
                }

                if (!string.IsNullOrWhiteSpace(request.Ruc))
                {
                    var ruc = request.Ruc.Trim();
                    query = query.Where(p => p.Proveedor.DocumentNumber == ruc);
                }
            }

            var pagos = await query
                .OrderBy(p => p.Proveedor.LegalName)
                .ThenBy(p => p.Code)
                .ToListAsync(cancellationToken);

            return pagos
                .SelectMany(p => p.Detalles.OrderBy(d => d.Id).Select(d => new ListadoPagoProveedorRowResponse
                {
                    PagoCode = p.Code,
                    ProveedorRuc = p.Proveedor?.DocumentNumber ?? string.Empty,
                    ProveedorName = p.Proveedor?.LegalName ?? string.Empty,
                    FechaEmision = p.FechaEmision,
                    NumeroCheque = p.NumeroCheque,
                    BancoCode = p.BancoCode,
                    BancoName = p.Banco?.Name ?? string.Empty,
                    NumeroComprobanteRetencion = p.NumeroComprobanteRetencion,
                    MonedaCode = p.MonedaCode,
                    Estado = p.Estado,
                    ComprobanteCajaBancosImpreso = p.ComprobanteCajaBancosImpreso,
                    ProveedorCode = d.ProveedorCode,
                    ComprobantePagoCode = d.ComprobantePagoCode,
                    NumeroDocumento = d.NumeroDocumento,
                    FechaEmisionDocumento = d.DocumentoCompra?.FechaEmision ?? default,
                    FechaVencimientoDocumento = d.DocumentoCompra?.FechaVencimiento ?? default,
                    TotalSol = d.TotalSol,
                    PendienteSol = d.PendienteSol,
                    PagoSol = d.PagoSol,
                    AsientoProvisionCode = d.AsientoProvisionCode,
                    Detraccion = d.Detraccion,
                    BaseRetencionSol = d.BaseRetencionSol,
                    MontoRetenidoSol = d.MontoRetenidoSol
                }))
                .ToList();
        }
    }
}
