using Cepheus.Application.Features.Rrhh.Catalogos.Afps.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.GetAfpByCode
{
    public record GetAfpByCodeQuery(string Code) : IRequest<AfpResponse>;
}