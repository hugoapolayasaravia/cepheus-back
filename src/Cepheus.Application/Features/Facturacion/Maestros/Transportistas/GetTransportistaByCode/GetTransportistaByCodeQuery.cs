using Cepheus.Application.Features.Facturacion.Maestros.Transportistas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.GetTransportistaByCode
{
    public record GetTransportistaByCodeQuery(string Code) : IRequest<TransportistaResponse>;
}
