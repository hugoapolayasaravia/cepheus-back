using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.DeleteTrabajadorSalud
{
    public record DeleteTrabajadorSaludCommand(long Id) : IRequest;
}
