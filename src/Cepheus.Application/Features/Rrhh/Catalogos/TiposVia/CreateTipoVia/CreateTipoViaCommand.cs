using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.CreateTipoVia
{
    public record CreateTipoViaCommand(
        string Name,
        string? Abbreviation
    ) : IRequest<TipoViaResponse>;
}