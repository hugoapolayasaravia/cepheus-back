using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.GetOportunidadByCode
{
    public record GetOportunidadByCodeQuery(string Code) : IRequest<OportunidadResponse>;
}
