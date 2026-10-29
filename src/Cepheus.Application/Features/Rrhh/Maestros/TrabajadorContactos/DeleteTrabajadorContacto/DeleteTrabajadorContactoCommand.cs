using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.DeleteTrabajadorContacto
{
    public record DeleteTrabajadorContactoCommand(int Id) : IRequest;
}