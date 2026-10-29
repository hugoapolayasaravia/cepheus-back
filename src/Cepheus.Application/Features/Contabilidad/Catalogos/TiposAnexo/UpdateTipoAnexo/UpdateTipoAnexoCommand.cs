using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.UpdateTipoAnexo
{
    public record UpdateTipoAnexoCommand(
        string Code,
        string Name,
        string? Flag,
        byte[] RowVersion
    ) : IRequest<TipoAnexoResponse>;
}
