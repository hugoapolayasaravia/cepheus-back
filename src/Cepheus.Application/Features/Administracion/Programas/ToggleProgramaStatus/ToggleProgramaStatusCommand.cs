using MediatR;

namespace Cepheus.Application.Features.Administracion.Programas.ToggleProgramaStatus
{
    public record ToggleProgramaStatusCommand(int Id) : IRequest<bool>;
}
