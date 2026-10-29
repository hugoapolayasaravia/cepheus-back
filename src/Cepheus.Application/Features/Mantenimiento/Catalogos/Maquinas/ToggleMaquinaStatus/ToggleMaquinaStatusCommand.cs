using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.ToggleMaquinaStatus
{
    public record ToggleMaquinaStatusCommand(string Code) : IRequest<bool>;
}
