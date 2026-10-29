using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.UpdateTrabajadorDocumento
{
    public record UpdateTrabajadorDocumentoCommand(
        int Id,
        string TipoDocumentoCode,
        string DocumentNumber,
        bool IsPrimary
    ) : IRequest<TrabajadorDocumentoResponse>;
}