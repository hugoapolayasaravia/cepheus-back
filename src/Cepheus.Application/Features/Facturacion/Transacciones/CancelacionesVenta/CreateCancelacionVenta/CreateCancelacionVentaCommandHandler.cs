using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.CreateCancelacionVenta
{
    public class CreateCancelacionVentaCommandHandler
        : IRequestHandler<CreateCancelacionVentaCommand, CancelacionVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCancelacionVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CancelacionVentaResponse> Handle(
            CreateCancelacionVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = LetraVentaRules.Normalize(request.PlantaCode);
            var clienteCode = request.ClienteCode.Trim();
            var cobradorCode = request.CobradorCode.Trim();
            var moneda = request.MonedaCode.Trim().ToUpperInvariant();
            var tipoPago = request.TipoPagoCode.Trim();
            var date = request.Date.Date;

            // Legacy: la fecha no puede ser mayor a la fecha de proceso.
            if (date > DateTime.Today)
            {
                throw new InvalidOperationException("La fecha de cancelación no puede ser mayor a la fecha de proceso.");
            }

            if (tipoPago == CancelacionTipoPago.LetrasVarias)
            {
                throw new InvalidOperationException(
                    "Las Letras Varias (tipo de pago 11) se registran con la opción de generar letras varias.");
            }

            if (!await _uow.Facturacion.Catalogos.TiposPago.Query().AnyAsync(t => t.Code == tipoPago && t.IsActive, cancellationToken))
            {
                throw new InvalidOperationException($"El tipo de pago {tipoPago} no existe o está inactivo.");
            }

            if (!await _uow.Comunes.Plantas.Query().AnyAsync(p => p.Code == plantaCode, cancellationToken))
                throw new KeyNotFoundException($"Planta {plantaCode} no encontrada.");
            if (!await _uow.Facturacion.Maestros.Clientes.Query().AnyAsync(c => c.Code == clienteCode, cancellationToken))
                throw new KeyNotFoundException($"Cliente {clienteCode} no encontrado.");
            if (!await _uow.Facturacion.Maestros.Cobradores.Query().AnyAsync(c => c.Code == cobradorCode && c.IsActive, cancellationToken))
                throw new InvalidOperationException($"El cobrador {cobradorCode} no existe o está inactivo.");
            if (!await _uow.Comunes.Monedas.Query().AnyAsync(m => m.Code == moneda && m.IsActive, cancellationToken))
                throw new InvalidOperationException($"La moneda {moneda} no existe o está inactiva.");

            var isCanje = tipoPago == CancelacionTipoPago.Canje;
            var isCheque = CancelacionTipoPago.GeneraCheque(tipoPago);
            var isLetra = CancelacionTipoPago.GeneraLetra(tipoPago);

            var paymentNumber = string.IsNullOrWhiteSpace(request.PaymentNumber) ? null : request.PaymentNumber.Trim().ToUpperInvariant();
            var accountNumber = string.IsNullOrWhiteSpace(request.AccountNumber) ? null : request.AccountNumber.Trim();
            var bancoDoc = string.IsNullOrWhiteSpace(request.BancoDocCode) ? null : request.BancoDocCode.Trim().ToUpperInvariant();
            var bancoDep = string.IsNullOrWhiteSpace(request.BancoDepCode) ? null : request.BancoDepCode.Trim().ToUpperInvariant();
            var operationNumber = string.IsNullOrWhiteSpace(request.OperationNumber) ? null : request.OperationNumber.Trim().ToUpperInvariant();
            var isDeferred = request.IsDeferred;
            var deferredDate = request.DeferredDate?.Date;
            var operationDate = request.OperationDate?.Date;
            var total = DocumentoVentaRules.Round2(request.TotalAmount);

            // ---- Reglas por tipo de pago (legacy ue_set_btn_grb_data)
            if (isCanje)
            {
                total = 0m;
            }
            else
            {
                if (total <= 0)
                {
                    throw new InvalidOperationException("No se puede cancelar con monto 0.");
                }

                if (bancoDep is null)
                {
                    throw new InvalidOperationException("Indique el banco de depósito.");
                }

                var sinBanco = bancoDep.All(ch => ch == '0');

                if (!sinBanco)
                {
                    if (!await _uow.Comunes.Bancos.Query().AnyAsync(b => b.Code == bancoDep, cancellationToken))
                        throw new InvalidOperationException($"El banco de depósito {bancoDep} no existe.");

                    if (!isLetra && accountNumber is null)
                        throw new InvalidOperationException("Cuenta del banco de depósito no seleccionada.");
                }
            }

            if (isCheque)
            {
                if (paymentNumber is null)
                    throw new InvalidOperationException("Indique el número del cheque.");
                if (bancoDoc is null)
                    throw new InvalidOperationException("Indique el banco del cheque.");
                if (!await _uow.Comunes.Bancos.Query().AnyAsync(b => b.Code == bancoDoc, cancellationToken))
                    throw new InvalidOperationException($"El banco del cheque {bancoDoc} no existe.");

                if (tipoPago == CancelacionTipoPago.ChequeDiferido)
                {
                    isDeferred = true;

                    if (deferredDate is null)
                        throw new InvalidOperationException("Ingrese la fecha diferida / presentación.");
                    if (operationDate is null)
                        throw new InvalidOperationException("Ingrese la fecha de operación / giro.");

                    // Legacy: la fecha de la cancelación es la fecha de presentación del cheque.
                    date = deferredDate.Value;
                }
                else if (isDeferred && deferredDate is null)
                {
                    throw new InvalidOperationException("Ingrese la fecha diferida del cheque.");
                }
            }
            else
            {
                isDeferred = false;
            }

            if (isLetra)
            {
                if (deferredDate is null)
                    throw new InvalidOperationException("Ingrese la fecha de vencimiento de la letra.");

                paymentNumber = null; // lo asigna el sistema
            }

            await CancelacionVentaRules.EnsurePeriodOpenAsync(_uow, date, cancellationToken);

            var rate = await DocumentoVentaRules.ResolveExchangeRateAsync(_uow, moneda, date, cancellationToken);

            string code;

            try
            {
                code = await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var newCode = await CancelacionCodigoGenerator.NextAsync(_uow, ct);

                    var cancelacion = new CancelacionVenta
                    {
                        Code = newCode,
                        PlantaCode = plantaCode,
                        ClienteCode = clienteCode,
                        Date = date,
                        CobradorCode = cobradorCode,
                        MonedaCode = moneda,
                        ExchangeRate = rate,
                        TipoPagoCode = tipoPago,
                        PaymentNumber = paymentNumber,
                        AccountNumber = accountNumber,
                        BancoDocCode = bancoDoc,
                        BancoDepCode = bancoDep,
                        IsDeferred = isDeferred,
                        DeferredDate = deferredDate,
                        OperationNumber = operationNumber,
                        OperationDate = operationDate,
                        Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                        TotalAmount = total,
                        UsedAmount = 0m,
                        DifferenceAmount = 0m,
                        AvailableAmount = total,
                        Status = EstadoCancelacion.Pendiente
                    };

                    await _uow.Facturacion.Transacciones.CancelacionesVenta.AddAsync(cancelacion, ct);

                    // Cheque (02, 07) o letra (03) que nace con la cancelación.
                    await CancelacionVentaInstrumentoService.CreateAsync(_uow, cancelacion, ct);

                    if (request.Documents.Count > 0)
                    {
                        await CancelacionVentaDocumentosService.AddAsync(_uow, cancelacion, request.Documents, ct);
                        await CancelacionVentaRules.EnsureWithinTotalAsync(_uow, cancelacion, ct);
                    }

                    await _uow.SaveChangesAsync(ct);

                    if (cancelacion.Details.Count > 0)
                    {
                        await CancelacionVentaRules.RecalculateAsync(_uow, cancelacion, apply: false, ct);
                        await _uow.SaveChangesAsync(ct);
                    }

                    return newCode;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "Alguno de los documentos fue modificado por otro proceso. Vuelva a consultar los pendientes e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await CancelacionVentaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
