using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.CreateTrabajadorDocumento
{
    public record CreateTrabajadorDocumentoCommand(
        string TrabajadorCode,
        string TipoDocumentoCode,
        string DocumentNumber,
        bool IsPrimary
    ) : IRequest<TrabajadorDocumentoResponse>;
}