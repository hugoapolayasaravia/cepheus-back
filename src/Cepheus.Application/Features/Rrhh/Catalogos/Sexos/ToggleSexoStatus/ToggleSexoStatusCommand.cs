using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.ToggleSexoStatus
{
    public record ToggleSexoStatusCommand(string Code) : IRequest<bool>;
}