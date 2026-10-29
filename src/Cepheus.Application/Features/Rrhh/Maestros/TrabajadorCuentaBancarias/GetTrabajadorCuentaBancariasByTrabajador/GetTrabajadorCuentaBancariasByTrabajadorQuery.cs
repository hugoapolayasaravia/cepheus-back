using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.GetTrabajadorCuentaBancariasByTrabajador
{
    public record GetTrabajadorCuentaBancariasByTrabajadorQuery(string TrabajadorCode) : IRequest<List<TrabajadorCuentaBancariaResponse>>;
}
