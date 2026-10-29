using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.GetObraByCode
{
    public record GetObraByCodeQuery(string ClienteCode, string Code) : IRequest<ObraResponse>;
}
