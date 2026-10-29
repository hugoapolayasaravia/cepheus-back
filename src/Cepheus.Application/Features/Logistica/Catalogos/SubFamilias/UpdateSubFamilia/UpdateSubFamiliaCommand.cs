using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.UpdateSubFamilia
{
    // FamiliaCode NO se edita acá a propósito — reasignar una subfamilia a otra
    // familia es una operación estructural distinta (mismo criterio que
    // UpdateSubmoduloCommand no permite editar ModuloId).
    public record UpdateSubFamiliaCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<SubFamiliaResponse>;
}