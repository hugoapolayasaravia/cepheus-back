using Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.CreateAlturaLosa
{
    public record CreateAlturaLosaCommand(
        string Name,
        decimal Value,
        decimal Width,
        string ProductoTipoCode,
        string ProductoCode,
        string? PolystyreneProductoTipoCode,
        string? PolystyreneProductoCode,
        decimal PolystyreneValue,
        decimal PolystyreneWidth
    ) : IRequest<AlturaLosaResponse>;
}
