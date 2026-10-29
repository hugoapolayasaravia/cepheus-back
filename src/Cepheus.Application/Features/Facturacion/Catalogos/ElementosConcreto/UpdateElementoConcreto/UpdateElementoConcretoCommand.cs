using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.UpdateElementoConcreto
{
    public record UpdateElementoConcretoCommand(
        string Code,
        string Name,
        bool UsesBalance,
        byte[] RowVersion
    ) : IRequest<ElementoConcretoResponse>;
}
