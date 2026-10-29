using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.CreateCargo
{
    public record CreateCargoCommand(
        string Name
    ) : IRequest<CargoResponse>;
}