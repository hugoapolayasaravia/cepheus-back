using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.EmitirNotaDebitoVenta
{
    /// <summary>
    /// Emite la nota de débito. Legacy (ue_set_btn_imp):
    ///   1. fc_genasiprovision("08", numero, "0") verifica que todos los productos tengan cuenta contable.
    ///   2. Tras confirmar la impresión: fc_genasiprovision(..., "1") genera el asiento de provisión
    ///      (con plantilla, proceso ProvisionVenta) y la nota pasa a Facturado con impresión "S".
    /// Todo en una transacción: si el asiento falla, la nota queda Pendiente.
    /// </summary>
    public class EmitirNotaDebitoVentaCommandHandler
        : IRequestHandler<EmitirNotaDebitoVentaCommand, NotaDebitoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public EmitirNotaDebitoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaDebitoVentaResponse> Handle(
            EmitirNotaDebitoVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaDebitoVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var nota = await _uow.Facturacion.Transacciones.NotasDebitoVenta.Query()
                        .Include(n => n.Details)
                        .Include(n => n.Cliente)
                        .FirstOrDefaultAsync(n => n.PlantaCode == plantaCode && n.Number == number, ct);

                    if (nota is null)
                    {
                        throw new KeyNotFoundException(
                            $"Nota de débito {NotaDebitoVentaRules.FormatKey(plantaCode, number)} no encontrada.");
                    }

                    if (nota.Status != EstadoNotaDebito.Pendiente)
                    {
                        throw new InvalidOperationException($"La nota de débito está {nota.Status}: solo se emite una Pendiente.");
                    }

                    if (nota.Details.Count == 0 || nota.TotalAmount <= 0)
                    {
                        throw new InvalidOperationException("La nota de débito no tiene líneas o su total es cero.");
                    }

                    if (!string.IsNullOrWhiteSpace(nota.ProvisionEntryNumber))
                    {
                        throw new InvalidOperationException(
                            $"La nota de débito ya tiene el asiento de provisión {nota.ProvisionEntryNumber.Trim()}.");
                    }

                    var asiento = await NotaDebitoVentaAsientoService.BuildAsync(_uow, nota, ct);

                    nota.ProvisionEntryNumber = asiento.Codigo;
                    nota.Status = EstadoNotaDebito.Facturado;
                    nota.IsPrinted = true;
                    nota.PrintedAt = DateTime.UtcNow;

                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La nota de débito fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await NotaDebitoVentaMapper.LoadAsync(_uow, plantaCode, number, cancellationToken))!;
        }
    }
}
