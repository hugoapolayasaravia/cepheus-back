using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.GetNotaDebitoVentaByKey
{
    public record GetNotaDebitoVentaByKeyQuery(string PlantaCode, string Number) : IRequest<NotaDebitoVentaResponse>;
}
