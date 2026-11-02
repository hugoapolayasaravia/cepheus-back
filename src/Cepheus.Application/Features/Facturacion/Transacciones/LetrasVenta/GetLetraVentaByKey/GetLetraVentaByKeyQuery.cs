using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GetLetraVentaByKey
{
    public record GetLetraVentaByKeyQuery(string PlantaCode, string Number) : IRequest<LetraVentaResponse>;
}
