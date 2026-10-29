using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.GetSubFamiliaByCode
{
    public record GetSubFamiliaByCodeQuery(string Code) : IRequest<SubFamiliaResponse>;
}