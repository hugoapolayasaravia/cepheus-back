using Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCuentas.CreateProveedorCuenta
{
    public record CreateProveedorCuentaCommand(
        string ProveedorCode,
        string BancoCode,
        AccountType AccountType,
        string AccountNumber,
        string? InterbankCode,
        string MonedaCode,
        bool IsPrimary
    ) : IRequest<ProveedorCuentaResponse>;
}
