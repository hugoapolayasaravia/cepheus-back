using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.UpdateParentesco
{
    public record UpdateParentescoCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<ParentescoResponse>;
}