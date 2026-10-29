using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.UpdateNivelTrabajador
{
    public record UpdateNivelTrabajadorCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<NivelTrabajadorResponse>;
}