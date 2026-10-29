using Cepheus.Application.Features.Rrhh.Catalogos.Afps.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.UpdateAfp
{
    public record UpdateAfpCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<AfpResponse>;
}