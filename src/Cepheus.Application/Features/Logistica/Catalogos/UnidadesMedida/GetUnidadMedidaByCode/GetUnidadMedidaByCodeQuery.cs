using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.GetUnidadMedidaByCode
{
    public record GetUnidadMedidaByCodeQuery(string Code) : IRequest<UnidadMedidaResponse>;
}