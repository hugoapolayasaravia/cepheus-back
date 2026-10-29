using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.ToggleSubCentroEjecutorStatus
{
    public record ToggleSubCentroEjecutorStatusCommand(string Code) : IRequest<bool>;
}
