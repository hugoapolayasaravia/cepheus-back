using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.GetSolicitudNotaCreditoByCode
{
    public record GetSolicitudNotaCreditoByCodeQuery(string Year, string Month, string Code)
        : IRequest<SolicitudNotaCreditoResponse>;
}
