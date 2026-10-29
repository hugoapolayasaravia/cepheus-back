using Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.GetCuentaBancariaByCode
{
    public record GetCuentaBancariaByCodeQuery(string BankCode, string AccountNumber) : IRequest<CuentaBancariaResponse>;
}
