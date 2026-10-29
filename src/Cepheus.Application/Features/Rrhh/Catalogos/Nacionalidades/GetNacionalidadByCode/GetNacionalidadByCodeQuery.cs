using Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Nacionalidades.GetNacionalidadByCode
{
    public record GetNacionalidadByCodeQuery(string Code) : IRequest<NacionalidadResponse>;
}