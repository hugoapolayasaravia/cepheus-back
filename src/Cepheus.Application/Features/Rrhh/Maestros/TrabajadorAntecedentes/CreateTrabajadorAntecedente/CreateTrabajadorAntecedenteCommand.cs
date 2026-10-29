using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.CreateTrabajadorAntecedente
{
    public record CreateTrabajadorAntecedenteCommand(
        string TrabajadorCode,
        bool TieneAntecedentes,
        string? Descripcion
    ) : IRequest<TrabajadorAntecedenteResponse>;
}
