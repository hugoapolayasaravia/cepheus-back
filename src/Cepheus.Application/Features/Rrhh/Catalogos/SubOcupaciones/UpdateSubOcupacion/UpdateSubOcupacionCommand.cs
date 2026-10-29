using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.UpdateSubOcupacion
{
    public record UpdateSubOcupacionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<SubOcupacionResponse>;
}