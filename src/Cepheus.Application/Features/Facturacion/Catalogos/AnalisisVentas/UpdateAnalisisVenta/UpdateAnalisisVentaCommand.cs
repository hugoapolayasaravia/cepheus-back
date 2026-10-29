using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.UpdateAnalisisVenta
{
    public record UpdateAnalisisVentaCommand(
        string Code,
        string Name,
        string? ShortName,
        string? SegmentoVentasCode,
        byte[] RowVersion
    ) : IRequest<AnalisisVentaResponse>;
}
