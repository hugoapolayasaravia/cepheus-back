using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.DeleteTrabajadorAntecedente
{
    public record DeleteTrabajadorAntecedenteCommand(long Id) : IRequest;
}
