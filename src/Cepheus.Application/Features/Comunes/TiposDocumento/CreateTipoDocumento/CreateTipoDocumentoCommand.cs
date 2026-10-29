using Cepheus.Application.Features.Comunes.TiposDocumento.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposDocumento.CreateTipoDocumento
{
    public record CreateTipoDocumentoCommand(
        string Code,
        string Name,
        string? ShortName,
        string? SunatCode

    ) : IRequest<TipoDocumentoResponse>;
}
