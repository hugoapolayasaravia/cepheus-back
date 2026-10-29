using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.ToggleSctrSaludStatus
{
    public record ToggleSctrSaludStatusCommand(string Code) : IRequest<bool>;
}