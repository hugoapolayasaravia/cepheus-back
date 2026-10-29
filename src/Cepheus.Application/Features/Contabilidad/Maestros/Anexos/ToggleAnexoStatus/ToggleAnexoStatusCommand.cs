using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.Anexos.ToggleAnexoStatus
{
    public record ToggleAnexoStatusCommand(string TipoAnexoCode, string Code) : IRequest<bool>;
}
