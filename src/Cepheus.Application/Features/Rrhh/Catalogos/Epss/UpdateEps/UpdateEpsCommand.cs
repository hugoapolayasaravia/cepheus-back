using Cepheus.Application.Features.Rrhh.Catalogos.Epss.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.UpdateEps
{
    public record UpdateEpsCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<EpsResponse>;
}