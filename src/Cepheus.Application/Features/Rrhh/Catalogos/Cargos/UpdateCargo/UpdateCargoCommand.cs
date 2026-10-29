using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.UpdateCargo
{
    public record UpdateCargoCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<CargoResponse>;
}