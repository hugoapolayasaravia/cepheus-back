using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.UpdateFlete
{
    public record UpdateFleteCommand(
        string Code,
        string Name,
        decimal Amount,
        bool IsDefault,
        byte[] RowVersion
    ) : IRequest<FleteResponse>;
}
