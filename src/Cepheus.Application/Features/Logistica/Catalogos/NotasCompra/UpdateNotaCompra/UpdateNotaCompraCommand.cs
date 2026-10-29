using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common.Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.UpdateNotaCompra
{
    public record UpdateNotaCompraCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<NotaCompraResponse>;
}