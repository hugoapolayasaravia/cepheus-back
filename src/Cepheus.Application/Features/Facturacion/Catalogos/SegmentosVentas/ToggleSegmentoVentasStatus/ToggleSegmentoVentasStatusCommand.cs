using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.ToggleSegmentoVentasStatus
{
    public record ToggleSegmentoVentasStatusCommand(string Code) : IRequest<bool>;
}
