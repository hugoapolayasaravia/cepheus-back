using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.CreateEstadoTrabajador
{
    public record CreateEstadoTrabajadorCommand(
        string Name
    ) : IRequest<EstadoTrabajadorResponse>;
}