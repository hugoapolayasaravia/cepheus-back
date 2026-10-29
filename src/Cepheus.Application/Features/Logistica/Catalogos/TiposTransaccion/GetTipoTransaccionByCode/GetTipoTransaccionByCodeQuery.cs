using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.GetTipoTransaccionByCode
{
    public record GetTipoTransaccionByCodeQuery(string Code) : IRequest<TipoTransaccionResponse>;
}
