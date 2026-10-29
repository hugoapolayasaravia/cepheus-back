using Cepheus.Application.Features.Facturacion.Maestros.Choferes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.GetChoferByCode
{
    public record GetChoferByCodeQuery(string TransportistaCode, string Code) : IRequest<ChoferResponse>;
}
