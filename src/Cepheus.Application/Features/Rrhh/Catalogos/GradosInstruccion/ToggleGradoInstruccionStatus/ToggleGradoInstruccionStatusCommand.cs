using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.GradosInstruccion.ToggleGradoInstruccionStatus
{
    public record ToggleGradoInstruccionStatusCommand(string Code) : IRequest<bool>;
}