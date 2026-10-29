using Cepheus.Application.Features.Rrhh.Catalogos.Areas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.GetAreaByCode
{
    public record GetAreaByCodeQuery(string Code) : IRequest<AreaResponse>;
}