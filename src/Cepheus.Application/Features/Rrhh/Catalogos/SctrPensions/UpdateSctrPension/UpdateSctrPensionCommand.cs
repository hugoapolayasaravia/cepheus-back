using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.UpdateSctrPension
{
    public record UpdateSctrPensionCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<SctrPensionResponse>;
}