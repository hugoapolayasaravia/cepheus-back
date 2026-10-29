using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.ToggleUnidadMedidaVentaStatus
{
    public record ToggleUnidadMedidaVentaStatusCommand(string Code) : IRequest<bool>;
}
