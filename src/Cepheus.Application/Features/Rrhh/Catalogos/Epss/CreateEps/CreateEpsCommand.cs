using Cepheus.Application.Features.Rrhh.Catalogos.Epss.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.CreateEps
{
    public record CreateEpsCommand(
        string Name
    ) : IRequest<EpsResponse>;
}