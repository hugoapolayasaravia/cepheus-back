using Cepheus.Application.Features.Logistica.Maestros.Vehiculos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.GetVehiculoByCode
{
    public record GetVehiculoByCodeQuery(string Code) : IRequest<VehiculoResponse>;
}
