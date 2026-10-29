using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.CreateAnalisisVenta
{
    public record CreateAnalisisVentaCommand(
        string Name,
        string? ShortName,
        string? SegmentoVentasCode
    ) : IRequest<AnalisisVentaResponse>;
}
