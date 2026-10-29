using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.UpdateUser
{
    public record UpdateUserCommand(
        int Id,
        string Email,
        string FirstName,
        string LastName,
        string? TrabajadorCode,
        byte[] RowVersion
    ) : IRequest<UserResponse>;
}
