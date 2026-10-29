using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.DeleteTrabajadorSindicato
{
    public record DeleteTrabajadorSindicatoCommand(long Id) : IRequest;
}
