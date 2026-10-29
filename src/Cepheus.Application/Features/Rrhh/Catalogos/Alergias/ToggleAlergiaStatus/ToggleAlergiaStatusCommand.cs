using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.ToggleAlergiaStatus
{
    public record ToggleAlergiaStatusCommand(string Code) : IRequest<bool>;
}