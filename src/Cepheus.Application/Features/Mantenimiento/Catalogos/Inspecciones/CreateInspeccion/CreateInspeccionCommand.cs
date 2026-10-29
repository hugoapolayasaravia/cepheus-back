using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.CreateInspeccion
{
    public record CreateInspeccionCommand(
        string Code,
        string Name
    ) : IRequest<InspeccionResponse>;
}
