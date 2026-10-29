using Cepheus.Application.Features.Comunes.TiposDocumento.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposDocumento.GetTipoDocumentoById
{
    public record GetTipoDocumentoByIdQuery(string Code) : IRequest<TipoDocumentoResponse>;
}
