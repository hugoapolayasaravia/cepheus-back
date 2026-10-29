using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GetChequeVentaByKey
{
    public record GetChequeVentaByKeyQuery(string ClienteCode, string BancoCode, string Number) : IRequest<ChequeVentaResponse>;
}
