using Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposTrabajador.GetTipoTrabajadorByCode
{
    public record GetTipoTrabajadorByCodeQuery(string Code) : IRequest<TipoTrabajadorResponse>;
}