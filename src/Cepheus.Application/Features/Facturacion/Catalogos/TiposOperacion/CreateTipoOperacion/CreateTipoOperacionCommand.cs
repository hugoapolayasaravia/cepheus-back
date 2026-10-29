using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.CreateTipoOperacion
{
    public record CreateTipoOperacionCommand(
        string Name
    ) : IRequest<TipoOperacionResponse>;
}
