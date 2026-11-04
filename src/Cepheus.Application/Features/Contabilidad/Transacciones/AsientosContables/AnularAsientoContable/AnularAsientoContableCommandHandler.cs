using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.AnularAsientoContable
{
    /// <summary>
    /// Anula un asiento (ver AsientoAnulacionService para las reglas).
    /// </summary>
    public class AnularAsientoContableCommandHandler
        : IRequestHandler<AnularAsientoContableCommand, AsientoContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public AnularAsientoContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AsientoContableResponse> Handle(
            AnularAsientoContableCommand request, CancellationToken cancellationToken)
        {
            await EnsureNoPerteneceACobranzaAsync(request.Codigo, cancellationToken);

            var asiento = await AsientoAnulacionService.AnularAsync(_uow, request.Codigo, cancellationToken);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El asiento, la nota de ingreso o el documento fue modificado por otro proceso. Intente nuevamente.");
            }

            return AsientoContableMapper.Map(asiento);
        }

        /// <summary>
        /// El asiento de una cancelación de venta, de una letra o de un cheque no se anula desde Contabilidad: al
        /// anularlo hay que devolver los documentos a por cobrar, el cheque/letra a su estado anterior y recalcular
        /// saldos, y eso lo hace el módulo dueño del asiento.
        /// </summary>
        private async Task EnsureNoPerteneceACobranzaAsync(string codigoAsiento, CancellationToken cancellationToken)
        {
            var (anio, mes, tipo, numero) = AsientoContableRules.ParseCodigo(codigoAsiento);
            var codigo = anio + mes + tipo + numero;
            var t = _uow.Facturacion.Transacciones;

            if (await t.CancelacionesVenta.Query().AsNoTracking().AnyAsync(
                    c => c.ProvisionEntryNumber == codigo || c.CancellationEntryNumber == codigo, cancellationToken))
            {
                throw new InvalidOperationException(
                    $"El asiento {codigo} pertenece a una cancelación de venta: anúlelo desde Cancelaciones (Anular asiento).");
            }

            if (await t.CobranzasDudosas.Query().AsNoTracking().AnyAsync(c => c.CanjeEntry == codigo, cancellationToken))
            {
                throw new InvalidOperationException(
                    $"El asiento {codigo} es el canje de una cobranza dudosa: elimine la cobranza dudosa del documento desde Cobranza Dudosa.");
            }

            if (await t.LetrasVenta.Query().AsNoTracking().AnyAsync(
                    l => l.ProvisionEntryNumber == codigo || l.CancellationEntryNumber == codigo, cancellationToken))
            {
                throw new InvalidOperationException(
                    $"El asiento {codigo} pertenece a una letra de venta: anúlelo desde la cancelación que la generó.");
            }

            if (await t.ChequesVenta.Query().AsNoTracking().AnyAsync(
                    c => c.ProvisionEntryNumber == codigo || c.ReboteEntryNumber == codigo
                         || c.CancellationEntryNumber == codigo, cancellationToken))
            {
                throw new InvalidOperationException(
                    $"El asiento {codigo} pertenece a un cheque de venta: anúlelo desde Cheques (anular operación) o desde su cancelación.");
            }
        }
    }
}
