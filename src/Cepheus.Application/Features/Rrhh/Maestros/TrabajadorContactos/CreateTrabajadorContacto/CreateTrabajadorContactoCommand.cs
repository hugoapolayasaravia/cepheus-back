using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.CreateTrabajadorContacto
{
    public record CreateTrabajadorContactoCommand(
        string TrabajadorCode,
        string Name,
        string? Phone,
        string? ParentescoCode,
        bool IsPrimary
    ) : IRequest<TrabajadorContactoResponse>;
}