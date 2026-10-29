using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.UpdateUnidadNegocio
{
    // A diferencia de los demás catálogos, acá sí se permite editar la
    // relación (ParentCode), porque reubicar una unidad de negocio en la
    // jerarquía es una operación legítima y frecuente de este catálogo en
    // particular — no es un cambio de PK, es el propósito del campo.
    public record UpdateUnidadNegocioCommand(
        string Code,
        string? Name,
        string? ParentCode,
        byte[] RowVersion
    ) : IRequest<UnidadNegocioResponse>;
}