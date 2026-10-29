using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.ToggleEstadoCivilStatus
{
    public record ToggleEstadoCivilStatusCommand(string Code) : IRequest<bool>;
}