using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.GetParentescoByCode
{
    public record GetParentescoByCodeQuery(string Code) : IRequest<ParentescoResponse>;
}