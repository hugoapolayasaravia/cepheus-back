using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.CreateOficina
{
    public record CreateOficinaCommand(
        string Name
    ) : IRequest<OficinaResponse>;
}