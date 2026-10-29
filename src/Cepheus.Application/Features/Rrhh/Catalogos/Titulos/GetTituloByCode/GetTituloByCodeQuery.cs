using Cepheus.Application.Features.Rrhh.Catalogos.Titulos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.GetTituloByCode
{
    public record GetTituloByCodeQuery(string Code) : IRequest<TituloResponse>;
}