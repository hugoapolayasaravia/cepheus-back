using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.UpdateInspeccion
{
    public record UpdateInspeccionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<InspeccionResponse>;
}
