using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.CreateAtributoConcreto
{
    public record CreateAtributoConcretoCommand(
        string AttributeType,
        string Name
    ) : IRequest<AtributoConcretoResponse>;
}
