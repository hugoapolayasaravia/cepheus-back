using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.UpdateTrabajadorRemuneracion
{
    public record UpdateTrabajadorRemuneracionCommand(
        long Id,
        string? TrabajadorCode,
        decimal SueldoBasico,
        string? MonedaCode,
        string? ModoPagoCode,
        byte[] RowVersion
    ) : IRequest<TrabajadorRemuneracionResponse>;
}
