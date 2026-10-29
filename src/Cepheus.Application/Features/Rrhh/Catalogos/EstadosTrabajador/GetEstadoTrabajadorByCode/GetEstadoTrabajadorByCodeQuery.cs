using Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosTrabajador.GetEstadoTrabajadorByCode
{
    public record GetEstadoTrabajadorByCodeQuery(string Code) : IRequest<EstadoTrabajadorResponse>;
}