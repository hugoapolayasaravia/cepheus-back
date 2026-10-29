using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.CreateParentesco
{
    public record CreateParentescoCommand(
        string Name
    ) : IRequest<ParentescoResponse>;
}