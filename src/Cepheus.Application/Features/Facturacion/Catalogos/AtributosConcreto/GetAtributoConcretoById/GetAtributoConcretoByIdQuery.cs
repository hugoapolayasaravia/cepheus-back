using Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.GetAtributoConcretoById
{
    public record GetAtributoConcretoByIdQuery(string Code) : IRequest<AtributoConcretoResponse>;
}
