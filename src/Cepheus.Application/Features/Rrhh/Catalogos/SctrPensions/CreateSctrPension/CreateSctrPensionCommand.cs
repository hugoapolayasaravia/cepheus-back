using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.CreateSctrPension
{
    public record CreateSctrPensionCommand(
        string Name
    ) : IRequest<SctrPensionResponse>;
}