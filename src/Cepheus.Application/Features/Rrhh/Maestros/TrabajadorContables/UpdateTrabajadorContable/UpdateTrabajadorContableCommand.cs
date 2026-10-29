using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.UpdateTrabajadorContable
{
    public record UpdateTrabajadorContableCommand(
        long Id,
        string? TrabajadorCode,
        int NumeroItem,
        string? CuentaContable,
        string? Tipo,
        decimal Porcentaje,
        byte[] RowVersion
    ) : IRequest<TrabajadorContableResponse>;
}
