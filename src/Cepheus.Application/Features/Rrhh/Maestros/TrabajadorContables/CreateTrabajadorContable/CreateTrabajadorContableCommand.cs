using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.CreateTrabajadorContable
{
    public record CreateTrabajadorContableCommand(
        string TrabajadorCode,
        int NumeroItem,
        string? CuentaContable,
        string? Tipo,
        decimal Porcentaje
    ) : IRequest<TrabajadorContableResponse>;
}
