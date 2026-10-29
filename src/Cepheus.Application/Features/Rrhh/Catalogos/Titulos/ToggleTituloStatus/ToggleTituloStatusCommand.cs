using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Titulos.ToggleTituloStatus
{
    public record ToggleTituloStatusCommand(string Code) : IRequest<bool>;
}