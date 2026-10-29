using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.CreateTrabajadorRemuneracion
{
    public record CreateTrabajadorRemuneracionCommand(
        string TrabajadorCode,
        decimal SueldoBasico,
        string? MonedaCode,
        string? ModoPagoCode
    ) : IRequest<TrabajadorRemuneracionResponse>;
}
