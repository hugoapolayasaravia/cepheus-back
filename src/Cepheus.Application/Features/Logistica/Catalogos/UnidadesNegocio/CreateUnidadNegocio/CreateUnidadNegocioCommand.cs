using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.CreateUnidadNegocio
{
    public record CreateUnidadNegocioCommand(
        string? Name,
        string? ParentCode
    ) : IRequest<UnidadNegocioResponse>;
}