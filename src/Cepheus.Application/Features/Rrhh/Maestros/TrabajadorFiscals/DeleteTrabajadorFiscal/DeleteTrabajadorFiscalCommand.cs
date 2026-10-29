using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.DeleteTrabajadorFiscal
{
    public record DeleteTrabajadorFiscalCommand(long Id) : IRequest;
}
