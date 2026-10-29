using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.CreateCuentaBancaria
{
    public record CreateCuentaBancariaCommand(
        string BankCode,
        string AccountNumber,
        string CurrencyCode,
        string? CheckStart,
        string? CheckEnd,
        string? CheckCurrent,
        string ControlFlag,
        string AccountCode
    ) : IRequest<CuentaBancariaResponse>;
}
