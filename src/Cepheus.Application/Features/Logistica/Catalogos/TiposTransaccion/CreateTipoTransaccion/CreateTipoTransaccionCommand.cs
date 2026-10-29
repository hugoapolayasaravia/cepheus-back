using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.CreateTipoTransaccion
{
    public record CreateTipoTransaccionCommand(
        string Code,
        string Name
    ) : IRequest<TipoTransaccionResponse>;
}
