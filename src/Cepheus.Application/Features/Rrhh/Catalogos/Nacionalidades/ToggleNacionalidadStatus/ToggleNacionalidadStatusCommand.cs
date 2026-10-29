using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.ToggleNacionalidadStatus
{
    public record ToggleNacionalidadStatusCommand(string Code) : IRequest<bool>;
}