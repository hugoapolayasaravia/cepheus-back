using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.Common;
using MediatR;
using System.Collections.Specialized;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.UpdateUnidadMedida
{
    public record UpdateUnidadMedidaCommand(
        string Code,
        string Name,
        string IndDecimal,
        byte[] RowVersion
    ) : IRequest<UnidadMedidaResponse>;
}