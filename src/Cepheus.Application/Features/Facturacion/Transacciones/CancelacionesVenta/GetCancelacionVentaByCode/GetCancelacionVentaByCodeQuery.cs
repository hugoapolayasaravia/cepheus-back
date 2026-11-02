using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GetCancelacionVentaByCode
{
    /// <summary>Cancelación con sus documentos (legacy: pestañas "Cancelaciones" y "Detalle de Cancelaciones").</summary>
    public record GetCancelacionVentaByCodeQuery(string Code) : IRequest<CancelacionVentaResponse>;
}
