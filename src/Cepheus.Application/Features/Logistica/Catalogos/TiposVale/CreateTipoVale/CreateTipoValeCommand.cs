using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.CreateTipoVale
{
    public record CreateTipoValeCommand(
        string Name
    ) : IRequest<TipoValeResponse>;
}