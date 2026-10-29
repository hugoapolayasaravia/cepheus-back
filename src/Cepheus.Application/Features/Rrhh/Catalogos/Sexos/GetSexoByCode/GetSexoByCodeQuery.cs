using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.GetSexoByCode
{
    public record GetSexoByCodeQuery(string Code) : IRequest<SexoResponse>;
}