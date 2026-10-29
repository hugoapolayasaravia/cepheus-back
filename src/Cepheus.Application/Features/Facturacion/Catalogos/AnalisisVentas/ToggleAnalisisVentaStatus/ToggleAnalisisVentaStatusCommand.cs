using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.ToggleAnalisisVentaStatus
{
    public record ToggleAnalisisVentaStatusCommand(string Code) : IRequest<bool>;
}
