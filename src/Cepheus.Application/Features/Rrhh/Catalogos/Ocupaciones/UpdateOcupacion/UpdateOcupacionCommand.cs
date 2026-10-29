using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.UpdateOcupacion
{
    public record UpdateOcupacionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<OcupacionResponse>;
}