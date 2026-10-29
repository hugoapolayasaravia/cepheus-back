using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.GetUnidadMedidaVentaById
{
    public record GetUnidadMedidaVentaByIdQuery(string Code) : IRequest<UnidadMedidaVentaResponse>;
}
