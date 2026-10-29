using Cepheus.Application.Features.Logistica.Maestros.Transportistas.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.GetTransportistaByCode
{
    public record GetTransportistaByCodeQuery(string Code) : IRequest<TransportistaResponse>;
}
