using Cepheus.Application.Features.Logistica.Catalogos.Tramites.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.CreateTramite
{
    public record CreateTramiteCommand(
        string Name
    ) : IRequest<TramiteResponse>;
}