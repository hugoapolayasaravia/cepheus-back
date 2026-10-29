using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.UpdateTrabajadorAntecedente
{
    public record UpdateTrabajadorAntecedenteCommand(
        long Id,
        string? TrabajadorCode,
        bool TieneAntecedentes,
        string? Descripcion,
        byte[] RowVersion
    ) : IRequest<TrabajadorAntecedenteResponse>;
}
