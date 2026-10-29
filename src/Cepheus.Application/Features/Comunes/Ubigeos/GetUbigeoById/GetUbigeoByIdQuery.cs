using Cepheus.Application.Features.Comunes.Ubigeos.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Ubigeos.GetUbigeoById
{
    public record GetUbigeoByIdQuery(string Code) : IRequest<UbigeoResponse>;
}
