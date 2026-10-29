using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.CreateFlete
{
    public record CreateFleteCommand(
        string Name,
        decimal Amount,
        bool IsDefault
    ) : IRequest<FleteResponse>;
}
