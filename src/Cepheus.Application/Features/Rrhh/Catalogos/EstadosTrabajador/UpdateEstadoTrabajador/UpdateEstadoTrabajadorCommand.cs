using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.UpdateEstadoTrabajador
{
    public record UpdateEstadoTrabajadorCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<EstadoTrabajadorResponse>;
}