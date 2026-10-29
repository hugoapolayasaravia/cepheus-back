using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.ToggleSubFamiliaStatus
{
    public record ToggleSubFamiliaStatusCommand(string Code) : IRequest<bool>;
}