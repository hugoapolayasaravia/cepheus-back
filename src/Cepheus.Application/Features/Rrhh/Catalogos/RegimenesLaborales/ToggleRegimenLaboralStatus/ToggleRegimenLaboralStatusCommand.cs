using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.ToggleRegimenLaboralStatus
{
    public record ToggleRegimenLaboralStatusCommand(string Code) : IRequest<bool>;
}