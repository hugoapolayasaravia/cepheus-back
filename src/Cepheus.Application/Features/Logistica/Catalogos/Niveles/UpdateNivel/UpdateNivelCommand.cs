using Cepheus.Application.Features.Logistica.Catalogos.Niveles.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.UpdateNivel
{
    public record UpdateNivelCommand(
        string Code,
        string Name,
        DateTime FechaInicio,
        DateTime? FechaFin,
        byte[] RowVersion
    ) : IRequest<NivelResponse>;
}
