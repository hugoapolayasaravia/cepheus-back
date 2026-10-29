using Cepheus.Application.Features.Rrhh.Catalogos.Areas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.CreateArea
{
    public record CreateAreaCommand(
        string Name
    ) : IRequest<AreaResponse>;
}