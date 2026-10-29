using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.CreateTipoTrabajador
{
    public record CreateTipoTrabajadorCommand(
        string Name
    ) : IRequest<TipoTrabajadorResponse>;
}