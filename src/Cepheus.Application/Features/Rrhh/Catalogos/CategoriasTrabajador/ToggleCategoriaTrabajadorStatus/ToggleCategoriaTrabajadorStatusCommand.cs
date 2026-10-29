using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.CategoriasTrabajador.ToggleCategoriaTrabajadorStatus
{
    public record ToggleCategoriaTrabajadorStatusCommand(string Code) : IRequest<bool>;
}