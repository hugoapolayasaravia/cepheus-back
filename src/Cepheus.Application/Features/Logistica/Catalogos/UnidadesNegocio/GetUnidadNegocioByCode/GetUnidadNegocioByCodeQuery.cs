using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.GetUnidadNegocioByCode
{
    public record GetUnidadNegocioByCodeQuery(string Code) : IRequest<UnidadNegocioResponse>;
}