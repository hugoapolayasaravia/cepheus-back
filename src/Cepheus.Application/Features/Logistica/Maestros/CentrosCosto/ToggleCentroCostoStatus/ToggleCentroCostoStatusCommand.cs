using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.CentrosCosto.ToggleCentroCostoStatus
{
    public record ToggleCentroCostoStatusCommand(string Code) : IRequest<bool>;
}
