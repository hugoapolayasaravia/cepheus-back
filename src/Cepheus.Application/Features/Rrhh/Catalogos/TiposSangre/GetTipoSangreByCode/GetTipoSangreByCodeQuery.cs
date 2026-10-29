using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.GetTipoSangreByCode
{
    public record GetTipoSangreByCodeQuery(string Code) : IRequest<TipoSangreResponse>;
}