using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.UpdateOportunidad
{
    public record UpdateOportunidadCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<OportunidadResponse>;
}
