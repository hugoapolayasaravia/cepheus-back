using Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposVia.UpdateTipoVia
{
    public record UpdateTipoViaCommand(
        string Code,
        string Name,
        string? Abbreviation,
        byte[] RowVersion
    ) : IRequest<TipoViaResponse>;
}