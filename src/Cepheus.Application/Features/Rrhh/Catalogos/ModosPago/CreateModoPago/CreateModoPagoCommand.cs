using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.CreateModoPago
{
    public record CreateModoPagoCommand(
        string Name
    ) : IRequest<ModoPagoResponse>;
}