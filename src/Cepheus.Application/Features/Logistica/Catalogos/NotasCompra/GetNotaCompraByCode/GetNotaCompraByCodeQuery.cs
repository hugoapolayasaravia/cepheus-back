using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common.Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.GetNotaCompraByCode
{
    public record GetNotaCompraByCodeQuery(string Code) : IRequest<NotaCompraResponse>;
}