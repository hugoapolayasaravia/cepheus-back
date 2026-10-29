using Cepheus.Application.Features.Comunes.Ubigeos.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Ubigeos.UpdateUbigeo
{
    public record UpdateUbigeoCommand(
        string Code,
        string Department,
        string Province,
        string District,
        byte[] RowVersion
    ) : IRequest<UbigeoResponse>;
}
