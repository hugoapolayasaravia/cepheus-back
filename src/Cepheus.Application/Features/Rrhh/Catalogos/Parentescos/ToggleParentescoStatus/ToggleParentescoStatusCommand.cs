using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.ToggleParentescoStatus
{
    public record ToggleParentescoStatusCommand(string Code) : IRequest<bool>;
}