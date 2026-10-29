using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.UpdateSituacionEps
{
    public record UpdateSituacionEpsCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<SituacionEpsResponse>;
}