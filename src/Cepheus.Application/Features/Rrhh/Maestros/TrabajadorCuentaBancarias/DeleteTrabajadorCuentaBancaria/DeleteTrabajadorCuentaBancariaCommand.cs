using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.DeleteTrabajadorCuentaBancaria
{
    public record DeleteTrabajadorCuentaBancariaCommand(long Id) : IRequest;
}
