using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.GetFallaByCode
{
    public record GetFallaByCodeQuery(string Code) : IRequest<FallaResponse>;
}
