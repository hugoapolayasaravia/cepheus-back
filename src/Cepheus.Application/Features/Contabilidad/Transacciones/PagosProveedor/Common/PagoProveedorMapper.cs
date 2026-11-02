using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common
{
    public static class PagoProveedorMapper
    {
        public static async Task<PagoProveedorResponse?> LoadAsync(
            IUnitOfWork uow, string code, CancellationToken cancellationToken)
        {
            var pago = await uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .AsNoTracking()
                .Include(p => p.Proveedor)
                .Include(p => p.Banco)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.DocumentoCompra)
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken);

            return pago is null ? null : Map(pago);
        }

        public static PagoProveedorResponse Map(PagoProveedor p) => new()
        {
            Code = p.Code,
            NumeroCheque = p.NumeroCheque,
            NumeroControl = p.NumeroControl,
            BancoCode = p.BancoCode,
            BancoName = p.Banco?.Name ?? string.Empty,
            CuentaBancariaNumero = p.CuentaBancariaNumero,
            CuentaContableCode = p.CuentaContableCode,
            ProveedorCode = p.ProveedorCode,
            ProveedorRuc = p.Proveedor?.DocumentNumber ?? string.Empty,
            ProveedorName = p.Proveedor?.LegalName ?? string.Empty,
            PagarA = p.PagarA,
            NumeroComprobanteRetencion = p.NumeroComprobanteRetencion,
            FechaComprobanteRetencion = p.FechaComprobanteRetencion,
            FechaEmision = p.FechaEmision,
            MonedaCode = p.MonedaCode,
            PagoSol = p.PagoSol,
            RetencionSol = p.RetencionSol,
            TotalSol = p.TotalSol,
            PagoRealSol = p.PagoRealSol,
            PagoSolVenta = p.PagoSolVenta,
            MontoReferencial = p.MontoReferencial,
            AsientoCancelacionCode = p.AsientoCancelacionCode,
            AsientoAnulacionCode = p.AsientoAnulacionCode,
            Estado = p.Estado,
            Tipo = p.Tipo,
            FechaAnulacion = p.FechaAnulacion,
            ComprobanteCajaBancosImpreso = p.ComprobanteCajaBancosImpreso,
            ComprobanteRetencionImpreso = p.ComprobanteRetencionImpreso,
            Afecto = p.Afecto,
            Detraccion = p.Detraccion,
            Canje = p.Canje,
            DepositoElectronico = p.DepositoElectronico,
            Glosa = p.Glosa,
            PlantaCode = p.PlantaCode,
            ClienteCode = p.ClienteCode,
            FormaPagoCancelacionCode = p.FormaPagoCancelacionCode,
            ProveedorCuentaId = p.ProveedorCuentaId,
            Documentos = p.Detalles
                .OrderBy(d => d.Id)
                .Select(d => new PagoProveedorDetalleResponse
                {
                    Id = d.Id,
                    ComprobantePagoCode = d.ComprobantePagoCode,
                    NumeroDocumento = d.NumeroDocumento,
                    ProveedorCode = d.ProveedorCode,
                    FechaEmision = d.DocumentoCompra?.FechaEmision ?? default,
                    FechaVencimiento = d.DocumentoCompra?.FechaVencimiento ?? default,
                    TotalSol = d.TotalSol,
                    PendienteSol = d.PendienteSol,
                    PagoSol = d.PagoSol,
                    AsientoProvisionCode = d.AsientoProvisionCode,
                    Detraccion = d.Detraccion,
                    BaseRetencionSol = d.BaseRetencionSol,
                    MontoRetenidoSol = d.MontoRetenidoSol
                })
                .ToList(),
            CreatedAt = p.CreatedAt,
            CreatedBy = p.CreatedBy,
            UpdatedAt = p.UpdatedAt,
            UpdatedBy = p.UpdatedBy,
            RowVersion = p.RowVersion
        };
    }
}
