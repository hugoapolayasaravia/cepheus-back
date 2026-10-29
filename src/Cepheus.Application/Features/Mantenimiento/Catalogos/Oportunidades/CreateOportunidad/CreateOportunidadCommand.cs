using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.CreateOportunidad
{
    public record CreateOportunidadCommand(
        string Code,
        string Name
    ) : IRequest<OportunidadResponse>;
}
