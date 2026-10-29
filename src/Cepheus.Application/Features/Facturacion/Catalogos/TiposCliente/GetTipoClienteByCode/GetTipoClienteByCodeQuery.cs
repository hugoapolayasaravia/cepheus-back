using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.GetTipoClienteByCode
{
    public record GetTipoClienteByCodeQuery(string Code) : IRequest<TipoClienteResponse>;
}
