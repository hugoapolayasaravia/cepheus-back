using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.ToggleUnidadNegocioStatus
{
    public record ToggleUnidadNegocioStatusCommand(string Code) : IRequest<bool>;
}