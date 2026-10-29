using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.UpdateOficina
{
    public record UpdateOficinaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<OficinaResponse>;
}