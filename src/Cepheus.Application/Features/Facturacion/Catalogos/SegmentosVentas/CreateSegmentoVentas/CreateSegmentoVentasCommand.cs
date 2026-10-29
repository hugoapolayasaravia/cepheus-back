using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.CreateSegmentoVentas
{
    public record CreateSegmentoVentasCommand(
        string Name
    ) : IRequest<SegmentoVentasResponse>;
}
