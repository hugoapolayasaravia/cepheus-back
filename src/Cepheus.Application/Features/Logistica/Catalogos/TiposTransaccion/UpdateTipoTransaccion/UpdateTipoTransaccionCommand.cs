using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.UpdateTipoTransaccion
{
    public record UpdateTipoTransaccionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoTransaccionResponse>;
}
