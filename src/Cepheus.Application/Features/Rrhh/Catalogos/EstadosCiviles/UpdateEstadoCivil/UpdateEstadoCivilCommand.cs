using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.UpdateEstadoCivil
{
    public record UpdateEstadoCivilCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<EstadoCivilResponse>;
}