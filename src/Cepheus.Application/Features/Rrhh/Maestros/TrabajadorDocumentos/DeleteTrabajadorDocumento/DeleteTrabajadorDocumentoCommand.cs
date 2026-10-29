using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.DeleteTrabajadorDocumento
{
    public record DeleteTrabajadorDocumentoCommand(int Id) : IRequest;
}