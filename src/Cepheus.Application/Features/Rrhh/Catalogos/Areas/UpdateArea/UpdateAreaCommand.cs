using Cepheus.Application.Features.Rrhh.Catalogos.Areas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.UpdateArea
{
    public record UpdateAreaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<AreaResponse>;
}