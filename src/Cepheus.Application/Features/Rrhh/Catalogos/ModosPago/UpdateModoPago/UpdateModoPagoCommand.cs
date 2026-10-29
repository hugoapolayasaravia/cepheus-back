using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.UpdateModoPago
{
    public record UpdateModoPagoCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<ModoPagoResponse>;
}