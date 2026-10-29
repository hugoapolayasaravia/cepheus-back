using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.UpdateTipoVale
{
    public record UpdateTipoValeCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<TipoValeResponse>;
}