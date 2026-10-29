using Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.GetFleteByCode
{
    public record GetFleteByCodeQuery(string Code) : IRequest<FleteResponse>;
}
