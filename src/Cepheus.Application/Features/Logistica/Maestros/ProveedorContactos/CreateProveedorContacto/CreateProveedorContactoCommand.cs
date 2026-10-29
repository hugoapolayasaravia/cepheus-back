using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.CreateProveedorContacto
{
    public record CreateProveedorContactoCommand(
        string ProveedorCode,
        string FirstName,
        string? LastName,
        string? Position,
        string? Phone,
        string? MobilePhone,
        string? Email,
        bool IsPrimary
    ) : IRequest<ProveedorContactoResponse>;
}
