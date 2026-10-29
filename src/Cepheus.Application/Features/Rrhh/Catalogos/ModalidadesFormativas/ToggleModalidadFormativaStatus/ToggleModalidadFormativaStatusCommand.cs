using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModalidadesFormativas.ToggleModalidadFormativaStatus
{
    public record ToggleModalidadFormativaStatusCommand(string Code) : IRequest<bool>;
}