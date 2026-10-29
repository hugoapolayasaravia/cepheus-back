using Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.CreateTipoSangre
{
    public record CreateTipoSangreCommand(
        string Name
    ) : IRequest<TipoSangreResponse>;
}