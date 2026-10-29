using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.GetCargoByCode
{
    public record GetCargoByCodeQuery(string Code) : IRequest<CargoResponse>;
}