using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.GetClasificacionClienteByCode
{
    public record GetClasificacionClienteByCodeQuery(string Code) : IRequest<ClasificacionClienteResponse>;
}
