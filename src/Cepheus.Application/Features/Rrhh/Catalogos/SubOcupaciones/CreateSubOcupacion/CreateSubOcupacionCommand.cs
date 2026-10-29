using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.CreateSubOcupacion
{
    public record CreateSubOcupacionCommand(
        string Name
    ) : IRequest<SubOcupacionResponse>;
}