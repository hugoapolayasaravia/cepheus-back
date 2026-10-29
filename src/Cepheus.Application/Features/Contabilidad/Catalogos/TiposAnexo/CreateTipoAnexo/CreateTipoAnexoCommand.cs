using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.CreateTipoAnexo
{
    public record CreateTipoAnexoCommand(
        string Name,
        string? Flag
    ) : IRequest<TipoAnexoResponse>;
}
