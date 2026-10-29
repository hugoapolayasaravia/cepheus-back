using Cepheus.Application.Features.Administracion.Users.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.GetUserById
{
    public record GetUserByIdQuery(int Id) : IRequest<UserResponse>;
}
