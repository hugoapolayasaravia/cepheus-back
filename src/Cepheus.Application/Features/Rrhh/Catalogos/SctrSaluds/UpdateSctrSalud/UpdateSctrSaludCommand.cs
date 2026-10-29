using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.UpdateSctrSalud
{
    public record UpdateSctrSaludCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<SctrSaludResponse>;
}