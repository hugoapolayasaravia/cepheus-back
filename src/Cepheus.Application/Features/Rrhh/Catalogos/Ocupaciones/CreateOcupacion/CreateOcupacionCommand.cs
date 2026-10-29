using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.CreateOcupacion
{
    public record CreateOcupacionCommand(
        string Name
    ) : IRequest<OcupacionResponse>;
}