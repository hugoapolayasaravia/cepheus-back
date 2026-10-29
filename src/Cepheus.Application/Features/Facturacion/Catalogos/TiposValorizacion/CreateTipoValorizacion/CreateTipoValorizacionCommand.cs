using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.CreateTipoValorizacion
{
    public record CreateTipoValorizacionCommand(
        string Name,
        int Days
    ) : IRequest<TipoValorizacionResponse>;
}
