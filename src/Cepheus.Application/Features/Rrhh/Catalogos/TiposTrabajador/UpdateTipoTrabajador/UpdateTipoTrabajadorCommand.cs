using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.UpdateTipoTrabajador
{
    public record UpdateTipoTrabajadorCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoTrabajadorResponse>;
}