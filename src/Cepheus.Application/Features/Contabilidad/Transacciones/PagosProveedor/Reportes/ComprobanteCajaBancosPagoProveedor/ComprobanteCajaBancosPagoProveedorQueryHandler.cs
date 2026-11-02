using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Reportes.ComprobanteCajaBancosPagoProveedor
{
    public class ComprobanteCajaBancosPagoProveedorQueryHandler
        : IRequestHandler<ComprobanteCajaBancosPagoProveedorQuery, ComprobanteCajaBancosPagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public ComprobanteCajaBancosPagoProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ComprobanteCajaBancosPagoProveedorResponse> Handle(
            ComprobanteCajaBancosPagoProveedorQuery request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var pago = await _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .AsNoTracking()
                .Include(p => p.Proveedor)
                .Include(p => p.Banco)
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken)
                ?? throw new KeyNotFoundException("Codigo de Operacion no existe Verifique por favor ..");

            var asientoCode = (pago.AsientoCancelacionCode ?? string.Empty).Trim();
            if (asientoCode.Length != PagoProveedorRules.AsientoLength)
            {
                throw new InvalidOperationException("Asiento Contable no Generado...");
            }

            var anio = asientoCode[..4];
            var mes = asientoCode.Substring(4, 2);
            var tipo = asientoCode.Substring(6, 2);
            var numero = asientoCode.Substring(8, 5);

            var asiento = await _uow.Contabilidad.Transacciones.AsientosContables.Query()
                .AsNoTracking()
                .Include(a => a.Detalles)
                .FirstOrDefaultAsync(
                    a => a.Anio == anio && a.Mes == mes && a.TipoAsientoCode == tipo && a.Numero == numero,
                    cancellationToken)
                ?? throw new KeyNotFoundException($"Asiento {asientoCode} no encontrado.");

            var control = pago.NumeroControl?.Trim() ?? string.Empty;

            return new ComprobanteCajaBancosPagoProveedorResponse
            {
                PagoCode = pago.Code,
                AsientoCancelacionCode = asientoCode,
                NumeroComprobanteRetencion = pago.NumeroComprobanteRetencion,
                NumeroCheque = control.Length > 0 ? $"{pago.NumeroCheque}-{control}" : pago.NumeroCheque,
                FechaEmision = pago.FechaEmision,
                ProveedorCode = pago.ProveedorCode,
                ProveedorName = pago.Proveedor?.LegalName ?? string.Empty,
                PagarA = pago.PagarA,
                BancoCode = pago.BancoCode,
                BancoName = pago.Banco?.Name ?? string.Empty,
                MonedaCode = pago.MonedaCode,
                TotalSol = pago.TotalSol,
                RetencionSol = pago.RetencionSol,
                Glosa = pago.Glosa,
                AsientoAnio = asiento.Anio,
                AsientoMes = asiento.Mes,
                TipoAsientoCode = asiento.TipoAsientoCode,
                AsientoNumero = asiento.Numero,
                FechaProceso = asiento.FechaProceso,
                TipoCambio = asiento.TipoCambio,
                AsientoDescripcion = asiento.Descripcion,
                AsientoEstado = asiento.Estado,
                TotalCargosSol = asiento.TotalCargosSol,
                TotalAbonosSol = asiento.TotalAbonosSol,
                Lineas = asiento.Detalles
                    .OrderBy(d => d.ItemNumber)
                    .Select(d => new CajaBancosLineaResponse
                    {
                        ItemNumber = d.ItemNumber,
                        PlantaCode = d.PlantaCode,
                        CuentaContableCode = d.CuentaContableCode,
                        AnexoTipoCode = d.AnexoTipoCode,
                        AnexoCode = d.AnexoCode,
                        Descripcion = d.Descripcion,
                        NumeroDocumento = d.NumeroDocumento,
                        CargoSol = d.CargoSol,
                        AbonoSol = d.AbonoSol,
                        EsDestino = d.EsDestino,
                        TipoContabilidad = d.TipoContabilidad
                    })
                    .ToList()
            };
        }
    }
}
