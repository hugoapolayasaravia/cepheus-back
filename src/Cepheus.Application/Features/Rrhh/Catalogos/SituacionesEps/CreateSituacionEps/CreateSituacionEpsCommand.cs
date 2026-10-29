using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.CreateSituacionEps
{
    public record CreateSituacionEpsCommand(
        string Name
    ) : IRequest<SituacionEpsResponse>;
}