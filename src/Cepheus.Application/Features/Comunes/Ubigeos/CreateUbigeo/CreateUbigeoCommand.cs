using Cepheus.Application.Features.Comunes.Ubigeos.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Ubigeos.CreateUbigeo
{
    public record CreateUbigeoCommand(
        string Code,
        string Department,
        string Province,
        string District
    ) : IRequest<UbigeoResponse>;
}
