using Cepheus.Application.Features.Comunes.TiposDocumento.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposDocumento.UpdateTipoDocumento
{
    public record UpdateTipoDocumentoCommand(
        string Code,
        string Name,
        string? ShortName,
        string? SunatCode,
        byte[] RowVersion
    ) : IRequest<TipoDocumentoResponse>;
}
