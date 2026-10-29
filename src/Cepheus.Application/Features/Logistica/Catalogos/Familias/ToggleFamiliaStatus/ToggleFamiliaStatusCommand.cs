using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.ToggleFamiliaStatus
{
    public record ToggleFamiliaStatusCommand(string Code) : IRequest<bool>;
}