using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AtributosConcreto.ToggleAtributoConcretoStatus
{
    public record ToggleAtributoConcretoStatusCommand(string Code) : IRequest<bool>;
}
