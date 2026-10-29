using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.UpdateTrabajadorCuentaBancaria
{
    public record UpdateTrabajadorCuentaBancariaCommand(
        long Id,
        string? TrabajadorCode,
        string? TipoCuentaCode,
        string? BancoCode,
        string? MonedaCode,
        string? NumeroCuenta,
        string TipoOperacion,
        bool Principal,
        byte[] RowVersion
    ) : IRequest<TrabajadorCuentaBancariaResponse>;
}
