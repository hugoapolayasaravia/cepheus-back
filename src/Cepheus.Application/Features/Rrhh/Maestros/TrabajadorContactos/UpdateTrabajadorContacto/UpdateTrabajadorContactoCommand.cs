using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.UpdateTrabajadorContacto
{
    public record UpdateTrabajadorContactoCommand(
        int Id,
        string Name,
        string? Phone,
        string? ParentescoCode,
        bool IsPrimary
    ) : IRequest<TrabajadorContactoResponse>;
}