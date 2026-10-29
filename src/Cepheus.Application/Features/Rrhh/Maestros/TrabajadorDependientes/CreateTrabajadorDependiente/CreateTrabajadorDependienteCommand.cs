using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.CreateTrabajadorDependiente
{
    public record CreateTrabajadorDependienteCommand(
        string TrabajadorCode,
        string Nombre,
        string? ParentescoCode,
        DateTime? FechaNacimiento,
        string? Documento,
        bool Asegurado
    ) : IRequest<TrabajadorDependienteResponse>;
}
