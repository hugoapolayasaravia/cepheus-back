using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.CreateRegimenLaboral
{
    public record CreateRegimenLaboralCommand(
        string Name
    ) : IRequest<RegimenLaboralResponse>;
}