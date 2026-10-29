using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GenerarChequeVenta
{
    /// <summary>
    /// Liquida un cheque diferido (menú N del legacy): registra el número y la fecha de la operación
    /// (depósito/cobro) y el cheque pasa de Pendiente a Cobrado.
    /// </summary>
    public record GenerarChequeVentaCommand(
        string ClienteCode,
        string BancoCode,
        string Number,
        string OperationNumber,
        DateTime OperationDate,
        byte[] RowVersion
    ) : IRequest<ChequeVentaResponse>;
}
