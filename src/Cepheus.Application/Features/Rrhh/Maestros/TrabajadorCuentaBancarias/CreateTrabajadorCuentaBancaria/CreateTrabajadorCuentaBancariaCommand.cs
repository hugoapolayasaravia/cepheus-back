using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.CreateTrabajadorCuentaBancaria
{
    public record CreateTrabajadorCuentaBancariaCommand(
        string TrabajadorCode,
        string? TipoCuentaCode,
        string? BancoCode,
        string? MonedaCode,
        string? NumeroCuenta,
        string TipoOperacion,
        bool Principal
    ) : IRequest<TrabajadorCuentaBancariaResponse>;
}
