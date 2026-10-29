using Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.UpdateDocumentoCompra
{
    /// <summary>Actualiza solo la cabecera (la clave no cambia). Las líneas se administran con el detalle (siguiente entrega).</summary>
    public record UpdateDocumentoCompraCommand(
        string ComprobantePagoCode,
        string NumeroDocumento,
        string ProveedorCode,
        DateTime FechaEmision,
        DateTime FechaRecepcion,
        DateTime FechaVencimiento,
        string FormaPagoCode,
        string MonedaCode,
        DateOnly? TipoCambioFecha,
        decimal NoGravableSol,
        decimal ServicioSol,
        decimal IgvExteriorSol,
        string Glosa,
        string Origen,
        string? Procedencia,
        string? TipoBienCode,
        string? TipoOperacionCode,
        string? OperacionDetraccion,
        string? ConstanciaDetraccion,
        DateTime? FechaConstanciaDetraccion,
        string? NumeroDetraccion,
        DateTime? FechaEmisionDetraccion,
        decimal MontoDetraccionSol,
        string? DocumentoOrigenComprobantePagoCode,
        string? DocumentoOrigenNumero,
        string? OrdenCompraAdministracionCode,
        byte[] RowVersion,
        DateTime? FechaDocumento = null
    ) : IRequest<DocumentoCompraResponse>, IDocumentoCompraCabecera;
}
