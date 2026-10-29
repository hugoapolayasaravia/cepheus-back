using Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.GetEquipoByCode
{
    public record GetEquipoByCodeQuery(string Code) : IRequest<EquipoResponse>;
}
