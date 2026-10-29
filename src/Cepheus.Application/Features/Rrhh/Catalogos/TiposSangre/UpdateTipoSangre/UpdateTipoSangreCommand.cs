using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.UpdateTipoSangre
{
    public record UpdateTipoSangreCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoSangreResponse>;
}