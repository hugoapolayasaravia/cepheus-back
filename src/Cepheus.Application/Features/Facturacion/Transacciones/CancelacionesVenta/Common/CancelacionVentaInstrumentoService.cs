using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common
{
    /// <summary>
    /// Cheque o letra que nace de una cancelación (legacy ue_set_btn_grb_data: insert/update en RCheVentas para los
    /// tipos de pago 02 y 07, y en RLetVentas para el 03). Letras Varias (11) las crea su propio proceso.
    /// No guarda: quien llama hace el SaveChanges.
    /// </summary>
    public static class CancelacionVentaInstrumentoService
    {
        public static async Task CreateAsync(IUnitOfWork uow, CancelacionVenta c, CancellationToken cancellationToken)
        {
            if (CancelacionTipoPago.GeneraCheque(c.TipoPagoCode))
            {
                var number = c.PaymentNumber!.Trim();
                var banco = c.BancoDocCode!.Trim();

                // Legacy: el cheque no puede estar ya registrado en otra operación.
                var other = await uow.Facturacion.Transacciones.ChequesVenta.Query().AsNoTracking()
                    .Where(x => x.ClienteCode == c.ClienteCode && x.BancoCode == banco && x.Number == number)
                    .Select(x => x.CancelacionCode)
                    .FirstOrDefaultAsync(cancellationToken);

                if (other is not null)
                {
                    throw new InvalidOperationException($"El cheque {number} ya se encuentra registrado en la operación {other}.");
                }

                await uow.Facturacion.Transacciones.ChequesVenta.AddAsync(new ChequeVenta
                {
                    Number = number,
                    ClienteCode = c.ClienteCode,
                    BancoCode = banco,
                    PlantaCode = c.PlantaCode,
                    IssueDate = c.Date,
                    PaymentDate = c.DeferredDate ?? c.Date,
                    CobradorCode = c.CobradorCode,
                    MonedaCode = c.MonedaCode,
                    ExchangeRate = c.ExchangeRate,
                    TotalAmount = c.TotalAmount,
                    PendingAmount = c.TotalAmount,
                    CancelacionCode = c.Code,
                    IsDeferred = c.IsDeferred,
                    OperationNumber = c.OperationNumber,
                    OperationDate = c.OperationDate,
                    // Legacy: el cheque diferido queda Pendiente hasta liquidarse; el normal nace Cancelado (Cobrado).
                    Status = c.IsDeferred ? EstadoCheque.Pendiente : EstadoCheque.Cobrado
                }, cancellationToken);
            }
            else if (CancelacionTipoPago.GeneraLetra(c.TipoPagoCode))
            {
                var number = (await LetraVentaRules.ReserveNumbersAsync(uow, c.PlantaCode, 1, cancellationToken))[0];
                c.PaymentNumber = number;

                await uow.Facturacion.Transacciones.LetrasVenta.AddAsync(new LetraVenta
                {
                    PlantaCode = c.PlantaCode,
                    Number = number,
                    ClienteCode = c.ClienteCode,
                    BancoCode = string.IsNullOrWhiteSpace(c.BancoDocCode) ? null : c.BancoDocCode,
                    IssueDate = c.Date,
                    DueDate = c.DeferredDate ?? c.Date,
                    CobradorCode = c.CobradorCode,
                    MonedaCode = c.MonedaCode,
                    ExchangeRate = c.ExchangeRate,
                    TotalAmount = c.TotalAmount,
                    PendingAmount = c.TotalAmount,
                    Interest = 0m,
                    CancelacionCode = c.Code,
                    IsPrinted = false,
                    AcceptedByClient = false,
                    Status = EstadoLetra.Facturado
                }, cancellationToken);
            }
        }

        public static async Task<ChequeVenta?> FindChequeAsync(
            IUnitOfWork uow, CancelacionVenta c, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(c.PaymentNumber) || string.IsNullOrWhiteSpace(c.BancoDocCode))
            {
                return null;
            }

            var number = c.PaymentNumber.Trim();
            var banco = c.BancoDocCode.Trim();

            return await uow.Facturacion.Transacciones.ChequesVenta.Query()
                .FirstOrDefaultAsync(
                    x => x.ClienteCode == c.ClienteCode && x.BancoCode == banco && x.Number == number
                         && x.CancelacionCode == c.Code,
                    cancellationToken);
        }

        public static async Task<LetraVenta?> FindLetraAsync(
            IUnitOfWork uow, CancelacionVenta c, CancellationToken cancellationToken)
            => await uow.Facturacion.Transacciones.LetrasVenta.Query()
                .FirstOrDefaultAsync(x => x.CancelacionCode == c.Code, cancellationToken);

        /// <summary>Refleja en el cheque o la letra los cambios de la cabecera (legacy: update de RCheVentas / RLetVentas).</summary>
        public static async Task SyncAsync(IUnitOfWork uow, CancelacionVenta c, CancellationToken cancellationToken)
        {
            if (CancelacionTipoPago.GeneraCheque(c.TipoPagoCode))
            {
                var cheque = await FindChequeAsync(uow, c, cancellationToken);
                if (cheque is null)
                {
                    return;
                }

                cheque.PlantaCode = c.PlantaCode;
                cheque.IssueDate = c.Date;
                cheque.PaymentDate = c.DeferredDate ?? c.Date;
                cheque.CobradorCode = c.CobradorCode;
                cheque.MonedaCode = c.MonedaCode;
                cheque.ExchangeRate = c.ExchangeRate;
                cheque.TotalAmount = c.TotalAmount;
                cheque.PendingAmount = c.TotalAmount;
                cheque.OperationNumber = c.OperationNumber;
                cheque.OperationDate = c.OperationDate;
                cheque.IsDeferred = c.IsDeferred;
            }
            else if (CancelacionTipoPago.GeneraLetra(c.TipoPagoCode))
            {
                var letra = await FindLetraAsync(uow, c, cancellationToken);
                if (letra is null)
                {
                    return;
                }

                letra.IssueDate = c.Date;
                letra.DueDate = c.DeferredDate ?? c.Date;
                letra.CobradorCode = c.CobradorCode;
                letra.MonedaCode = c.MonedaCode;
                letra.ExchangeRate = c.ExchangeRate;
                letra.TotalAmount = c.TotalAmount;
                letra.PendingAmount = c.TotalAmount;
            }
        }

        /// <summary>Elimina el cheque o la letra de la operación (legacy: delete from rcheventas / rletventas).</summary>
        public static async Task DeleteAsync(IUnitOfWork uow, CancelacionVenta c, CancellationToken cancellationToken)
        {
            if (CancelacionTipoPago.GeneraCheque(c.TipoPagoCode))
            {
                var cheques = await uow.Facturacion.Transacciones.ChequesVenta.Query()
                    .Where(x => x.CancelacionCode == c.Code)
                    .ToListAsync(cancellationToken);

                foreach (var cheque in cheques)
                {
                    uow.Facturacion.Transacciones.ChequesVenta.Remove(cheque);
                }
            }
            else if (c.TipoPagoCode is CancelacionTipoPago.Letras or CancelacionTipoPago.LetrasVarias)
            {
                var letras = await uow.Facturacion.Transacciones.LetrasVenta.Query()
                    .Where(x => x.CancelacionCode == c.Code)
                    .ToListAsync(cancellationToken);

                foreach (var letra in letras)
                {
                    uow.Facturacion.Transacciones.LetrasVenta.Remove(letra);
                }
            }
        }
    }
}
