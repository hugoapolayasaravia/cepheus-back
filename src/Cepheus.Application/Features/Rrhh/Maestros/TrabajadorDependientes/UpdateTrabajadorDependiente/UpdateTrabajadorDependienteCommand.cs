using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.UpdateTrabajadorDependiente
{
    public record UpdateTrabajadorDependienteCommand(
        long Id,
        string? TrabajadorCode,
        string Nombre,
        string? ParentescoCode,
        DateTime? FechaNacimiento,
        string? Documento,
        bool Asegurado,
        byte[] RowVersion
    ) : IRequest<TrabajadorDependienteResponse>;
}
