using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.CreateElementoConcreto
{
    public record CreateElementoConcretoCommand(
        string Name,
        bool UsesBalance = true
    ) : IRequest<ElementoConcretoResponse>;
}
