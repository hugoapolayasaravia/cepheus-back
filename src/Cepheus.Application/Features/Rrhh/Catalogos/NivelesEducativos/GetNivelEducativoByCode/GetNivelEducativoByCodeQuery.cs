using Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesEducativos.GetNivelEducativoByCode
{
    public record GetNivelEducativoByCodeQuery(string Code) : IRequest<NivelEducativoResponse>;
}