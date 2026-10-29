using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.CreateDocumentoCompra
{
    public class CreateDocumentoCompraCommandHandler : IRequestHandler<CreateDocumentoCompraCommand, DocumentoCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateDocumentoCompraCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DocumentoCompraResponse> Handle(
            CreateDocumentoCompraCommand request, CancellationToken cancellationToken)
        {
            var tdo = request.ComprobantePagoCode.Trim().ToUpperInvariant();
            var numero = request.NumeroDocumento.Trim();
            var prv = request.ProveedorCode.Trim().ToUpperInvariant();

            var existe = await _uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                .AnyAsync(d => d.ComprobantePagoCode == tdo && d.NumeroDocumento == numero && d.ProveedorCode == prv,
                    cancellationToken);

            if (existe)
            {
                throw new InvalidOperationException(
                    $"Ya existe el documento {tdo} {numero} del proveedor {prv}.");
            }

            var monedaCode = request.MonedaCode.Trim().ToUpperInvariant();
            var esSoles = monedaCode == DocumentoCompraRules.MonedaSoles;

            var doc = new DocumentoCompra
            {
                ComprobantePagoCode = tdo,
                NumeroDocumento = numero,
                ProveedorCode = prv,
                FechaEmision = request.FechaEmision.Date,
                FechaRecepcion = request.FechaRecepcion.Date,
                FechaDocumento = request.FechaDocumento?.Date,
                FechaVencimiento = request.FechaVencimiento.Date,
                FormaPagoCode = request.FormaPagoCode.Trim().ToUpperInvariant(),
                MonedaCode = monedaCode,
                TipoCambioFecha = esSoles ? null : request.TipoCambioFecha,
                NoGravableSol = request.NoGravableSol,
                ServicioSol = request.ServicioSol,
                IgvExteriorSol = request.IgvExteriorSol,
                Glosa = request.Glosa.Trim(),
                Origen = System.Enum.Parse<OrigenDocumentoCompra>(request.Origen, true),
                Procedencia = string.IsNullOrWhiteSpace(request.Procedencia)
                    ? null
                    : System.Enum.Parse<ProcedenciaDocumentoCompra>(request.Procedencia, true),
                Estado = EstadoDocumentoCompra.Pendiente,
                TipoBienCode = Clean(request.TipoBienCode),
                TipoOperacionCode = Clean(request.TipoOperacionCode),
                OperacionDetraccion = request.OperacionDetraccion?.Trim(),
                ConstanciaDetraccion = request.ConstanciaDetraccion?.Trim(),
                FechaConstanciaDetraccion = request.FechaConstanciaDetraccion?.Date,
                NumeroDetraccion = request.NumeroDetraccion?.Trim(),
                FechaEmisionDetraccion = request.FechaEmisionDetraccion?.Date,
                MontoDetraccionSol = request.MontoDetraccionSol,
                DocumentoOrigenComprobantePagoCode = Clean(request.DocumentoOrigenComprobantePagoCode),
                DocumentoOrigenNumero = request.DocumentoOrigenNumero?.Trim(),
                OrdenCompraAdministracionCode = Clean(request.OrdenCompraAdministracionCode),
                PagadoSol = 0
            };

            if (string.IsNullOrWhiteSpace(doc.DocumentoOrigenNumero))
            {
                doc.DocumentoOrigenNumero = null;
            }

            var item = 1;
            foreach (var line in request.Detalles)
            {
                doc.Detalles.Add(new DocumentoCompraDetalle
                {
                    ComprobantePagoCode = tdo,
                    NumeroDocumento = numero,
                    ProveedorCode = prv,
                    ItemNumber = item++,
                    PlantaCode = line.PlantaCode.Trim().ToUpperInvariant(),
                    CuentaContableCode = line.CuentaContableCode.Trim().ToUpperInvariant(),
                    AnexoTipoCode = Clean(line.AnexoTipoCode),
                    AnexoCode = Clean(line.AnexoCode),
                    Detalle = line.Detalle.Trim(),
                    Cantidad = line.Cantidad,
                    UnidadMedidaCode = line.UnidadMedidaCode.Trim().ToUpperInvariant(),
                    PrecioSol = line.PrecioSol,
                    ParcialSol = DocumentoCompraTotalsCalculator.CalculateLineTotal(line.Cantidad, line.PrecioSol),
                    NumeroGuiaVenta = line.NumeroGuiaVenta?.Trim(),
                    ProductoTipoCode = Clean(line.ProductoTipoCode),
                    ProductoCode = Clean(line.ProductoCode)
                });
            }

            await DocumentoCompraTotalsCalculator.RecalculateAsync(_uow, doc, cancellationToken);

            await _uow.Contabilidad.Transacciones.DocumentosCompra.AddAsync(doc, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var response = DocumentoCompraMapper.Map(doc);
            response.TipoCambioVenta = await DocumentoCompraTipoCambioResolver.GetVentaAsync(
                _uow, doc.MonedaCode, doc.TipoCambioFecha, cancellationToken);
            return response;
        }

        private static string? Clean(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }
}
