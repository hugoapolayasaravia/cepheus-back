using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.UpdateAlturaLosa
{
    public record UpdateAlturaLosaCommand(
        string Code,
        string Name,
        decimal Value,
        decimal Width,
        string ProductoTipoCode,
        string ProductoCode,
        string? PolystyreneProductoTipoCode,
        string? PolystyreneProductoCode,
        decimal PolystyreneValue,
        decimal PolystyreneWidth,
        byte[] RowVersion
    ) : IRequest<AlturaLosaResponse>;
}
