using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.DeleteTrabajadorSeguro
{
    public record DeleteTrabajadorSeguroCommand(long Id) : IRequest;
}
