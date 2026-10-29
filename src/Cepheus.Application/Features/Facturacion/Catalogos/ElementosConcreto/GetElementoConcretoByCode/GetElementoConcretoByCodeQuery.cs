using Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ElementosConcreto.GetElementoConcretoByCode
{
    public record GetElementoConcretoByCodeQuery(string Code) : IRequest<ElementoConcretoResponse>;
}
