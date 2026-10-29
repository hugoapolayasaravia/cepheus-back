using Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.NivelesTrabajador.GetNivelTrabajadorByCode
{
    public record GetNivelTrabajadorByCodeQuery(string Code) : IRequest<NivelTrabajadorResponse>;
}