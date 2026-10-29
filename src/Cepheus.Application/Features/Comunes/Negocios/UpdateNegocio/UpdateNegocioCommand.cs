using Cepheus.Application.Features.Comunes.Negocios.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Negocios.UpdateNegocio
{
    public record UpdateNegocioCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<NegocioResponse>;
}
