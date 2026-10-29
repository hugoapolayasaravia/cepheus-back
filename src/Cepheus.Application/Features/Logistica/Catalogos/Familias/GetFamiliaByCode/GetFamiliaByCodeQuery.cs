using Cepheus.Application.Features.Logistica.Catalogos.Familias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.GetFamiliaByCode
{
    public record GetFamiliaByCodeQuery(string Code) : IRequest<FamiliaResponse>;
}