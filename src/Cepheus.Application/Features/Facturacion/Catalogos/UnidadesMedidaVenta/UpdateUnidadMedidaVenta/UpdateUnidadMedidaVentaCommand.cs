using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.UpdateUnidadMedidaVenta
{
    public record UpdateUnidadMedidaVentaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<UnidadMedidaVentaResponse>;
}
