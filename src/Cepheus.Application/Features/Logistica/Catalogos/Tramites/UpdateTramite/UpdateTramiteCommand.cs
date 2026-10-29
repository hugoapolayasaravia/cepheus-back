using Cepheus.Application.Features.Logistica.Catalogos.Tramites.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.UpdateTramite
{
    public record UpdateTramiteCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TramiteResponse>;
}