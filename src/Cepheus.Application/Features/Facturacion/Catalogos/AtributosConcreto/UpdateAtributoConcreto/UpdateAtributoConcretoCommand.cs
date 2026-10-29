using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.UpdateAtributoConcreto
{
    public record UpdateAtributoConcretoCommand(
        string Code,
        string AttributeType,
        string Name,
        byte[] RowVersion
    ) : IRequest<AtributoConcretoResponse>;
}
