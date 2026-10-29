using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.CuentasBancarias.ToggleCuentaBancariaStatus
{
    public record ToggleCuentaBancariaStatusCommand(string BankCode, string AccountNumber) : IRequest<bool>;
}
