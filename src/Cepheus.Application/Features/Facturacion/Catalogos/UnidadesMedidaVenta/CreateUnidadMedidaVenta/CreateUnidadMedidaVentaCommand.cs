using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.CreateUnidadMedidaVenta
{
    public record CreateUnidadMedidaVentaCommand(
        string Code,
        string Name
    ) : IRequest<UnidadMedidaVentaResponse>;
}
