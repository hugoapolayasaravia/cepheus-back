using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.RegistrarCobranzaDudosa
{
    public class RegistrarCobranzaDudosaCommandHandler
        : IRequestHandler<RegistrarCobranzaDudosaCommand, CobranzaDudosaResponse>
    {
        private readonly IUnitOfWork _uow;

        public RegistrarCobranzaDudosaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CobranzaDudosaResponse> Handle(
            RegistrarCobranzaDudosaCommand request, CancellationToken cancellationToken)
        {
            var type = CobranzaDudosaRules.NormalizeType(request.TypeCode);
            var number = request.Number.Trim();
            var planta = CobranzaDudosaRules.PlantaOrNull(type, request.PlantaCode);
            var banco = CobranzaDudosaRules.BancoOrNull(type, request.BancoCode);
            var cliente = CobranzaDudosaRules.ClienteOrNull(request.ClienteCode);

            CobranzaDudosaRules.EnsureReference(type, number, request.PlantaCode, request.ClienteCode, request.BancoCode);

            string documentClienteCode = string.Empty;

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var doc = await CobranzaDocumentoService.LoadAsync(_uow, type, planta, number, cliente ?? string.Empty, banco, ct)
                        ?? throw new KeyNotFoundException($"Documento {type} {number} no registrado.");

                    if (doc.DoubtfulStatus != EstadoCobranzaDudosa.Normal)
                    {
                        throw new InvalidOperationException($"El documento {type} {number} ya fue enviado a cobranza dudosa.");
                    }

                    if (!CancelacionVentaRules.IsAsiento(doc.ProvisionEntry))
                    {
                        throw new InvalidOperationException($"El documento {type} {number} no tiene asiento de provisión generado.");
                    }

                    if (doc.Pending <= 0)
                    {
                        throw new InvalidOperationException($"El documento {type} {number} no tiene saldo pendiente.");
                    }

                    // Legacy fg_cambio(fecha, "V"): el tipo de cambio venta de la fecha de proceso (moneda base: 1).
                    var rate = await DocumentoVentaRules.ResolveExchangeRateAsync(_uow, doc.CurrencyCode, request.ProcessDate, ct);
                    var (soles, dollars) = CobranzaDudosaRules.Totals(doc, rate);

                    documentClienteCode = doc.ClienteCode;

                    await _uow.Facturacion.Transacciones.CobranzasDudosas.AddAsync(new CobranzaDudosa
                    {
                        TypeCode = type,
                        PlantaCode = CobranzaDudosaRules.UsaPlanta(type) ? doc.PlantaCode ?? string.Empty : string.Empty,
                        Number = doc.Number,
                        ClienteCode = doc.ClienteCode,
                        BancoCode = type == CobranzaTipoDocumento.Cheque ? doc.BancoCode ?? string.Empty : string.Empty,
                        ObraCode = CobranzaDudosaRules.SinObra(type) ? null : doc.ObraCode,
                        ProcessDate = request.ProcessDate.Date,
                        SystemDate = DateTime.Today,
                        TipoContabilidad = request.TipoContabilidad,
                        CurrencyCode = doc.CurrencyCode,
                        ExchangeRate = rate,
                        TotalSoles = soles,
                        TotalDollars = dollars,
                        ProvisionEntry = doc.ProvisionEntry!.Trim(),
                        CanjeEntry = null,
                        Status = EstadoRegistroCobranzaDudosa.Registrado
                    }, ct);

                    // Legacy: cobranzadudosa = 'S' (y TipCamCdu).
                    doc.SetDoubtful(EstadoCobranzaDudosa.Dudosa, rate);

                    await _uow.SaveChangesAsync(ct);
                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El documento fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }
            catch (DbUpdateException)
            {
                throw new InvalidOperationException($"El documento {type} {number} ya está registrado en cobranza dudosa.");
            }

            _uow.ClearTracking();

            return await CobranzaDudosaMapper.LoadAsync(
                _uow, type, planta, number, documentClienteCode, banco, cancellationToken);
        }
    }
}
