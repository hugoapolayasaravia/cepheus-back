using Cepheus.Application.Features.Comunes.Negocios.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Negocios.CreateNegocio
{
    /// <summary>
    /// Code se recibe del cliente (2 caracteres, código mnemotécnico
    /// asignado manualmente — igual criterio que UnidadMedida en Logística),
    /// no es un correlativo automático.
    /// </summary>
    public record CreateNegocioCommand(
        string Code,
        string Name
    ) : IRequest<NegocioResponse>;
}
