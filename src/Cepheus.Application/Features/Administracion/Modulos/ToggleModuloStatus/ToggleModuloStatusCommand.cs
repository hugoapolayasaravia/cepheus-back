using MediatR;

namespace Cepheus.Application.Features.Administracion.Modulos.ToggleModuloStatus
{
    public record ToggleModuloStatusCommand(int Id) : IRequest<bool>;
}
