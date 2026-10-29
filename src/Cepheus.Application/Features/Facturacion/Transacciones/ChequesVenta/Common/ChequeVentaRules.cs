using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common
{
    public static class ChequeVentaRules
    {
        public static string Normalize(string value) => value.Trim().ToUpperInvariant();

        public static string FormatKey(string clienteCode, string bancoCode, string number)
            => $"{bancoCode}/{clienteCode}/{number}";

        /// <summary>Legacy ue_valida_cheque: el cheque necesita el asiento de la cancelación que lo originó (13 caracteres).</summary>
        public static bool HasCancelacionEntry(ChequeVenta c)
            => !string.IsNullOrWhiteSpace(c.ProvisionEntryNumber) && c.ProvisionEntryNumber.Trim().Length == 13;

        /// <summary>Menú R del legacy: cheque Cobrado o Pendiente.</summary>
        public static bool CanRebotar(ChequeVenta c)
            => c.Status is EstadoCheque.Cobrado or EstadoCheque.Pendiente;

        /// <summary>Menú N del legacy: cheque diferido Pendiente.</summary>
        public static bool CanGenerar(ChequeVenta c)
            => c.Status == EstadoCheque.Pendiente && c.IsDeferred;

        /// <summary>
        /// Deshacer: cheque rebotado, o cheque diferido ya liquidado. Un cheque rebotado que ya se usó en otra
        /// cancelación (su saldo cambió) no se puede deshacer.
        /// </summary>
        public static bool CanAnularOperacion(ChequeVenta c)
            => (c.Status == EstadoCheque.Rebotado && c.PendingAmount == c.TotalAmount)
               || (c.IsDeferred && c.Status == EstadoCheque.Cobrado);

        public static async Task<ChequeVenta> GetTrackedAsync(
            IUnitOfWork uow, string clienteCode, string bancoCode, string number, CancellationToken cancellationToken)
            => await uow.Facturacion.Transacciones.ChequesVenta.Query()
                   .FirstOrDefaultAsync(
                       c => c.Number == number && c.ClienteCode == clienteCode && c.BancoCode == bancoCode,
                       cancellationToken)
               ?? throw new KeyNotFoundException(
                   $"Cheque {FormatKey(clienteCode, bancoCode, number)} no encontrado.");

        /// <summary>Concurrencia optimista: el cliente debe haber leído la última versión del cheque.</summary>
        public static void EnsureRowVersion(ChequeVenta cheque, byte[] rowVersion)
        {
            if (!cheque.RowVersion.SequenceEqual(rowVersion))
            {
                throw new InvalidOperationException(
                    "El cheque fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }
        }

        /// <summary>Legacy fg_cierre: la fecha no puede caer en un período contable cerrado.</summary>
        public static Task EnsurePeriodOpenAsync(IUnitOfWork uow, DateTime date, CancellationToken cancellationToken)
            => PeriodoContableRules.EnsureOpenAsync(uow, date.Year.ToString("0000"), date.Month.ToString("00"), cancellationToken);
    }
}
