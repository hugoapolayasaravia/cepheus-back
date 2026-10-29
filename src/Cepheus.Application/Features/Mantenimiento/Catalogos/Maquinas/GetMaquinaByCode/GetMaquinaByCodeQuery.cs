using Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.GetMaquinaByCode
{
    public record GetMaquinaByCodeQuery(string Code) : IRequest<MaquinaResponse>;
}
