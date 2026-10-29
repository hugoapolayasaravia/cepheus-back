using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Tecnicos.ToggleTecnicoStatus
{
    public record ToggleTecnicoStatusCommand(string TrabajadorCode) : IRequest<bool>;
}
