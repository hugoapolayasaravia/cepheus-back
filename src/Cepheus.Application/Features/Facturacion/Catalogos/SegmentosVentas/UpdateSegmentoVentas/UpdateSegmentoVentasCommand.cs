using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.UpdateSegmentoVentas
{
    public record UpdateSegmentoVentasCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<SegmentoVentasResponse>;
}
