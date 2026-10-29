using MediatR;

namespace Cepheus.Application.Features.Comunes.Bancos.ToggleBancoStatus
{
    public record ToggleBancoStatusCommand(string Code) : IRequest<bool>;
}
