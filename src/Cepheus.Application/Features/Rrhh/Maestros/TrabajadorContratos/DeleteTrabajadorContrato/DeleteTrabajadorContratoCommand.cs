using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.DeleteTrabajadorContrato
{
    public record DeleteTrabajadorContratoCommand(long Id) : IRequest;
}
