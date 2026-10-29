using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Common
{
    /// <summary>
    /// Asientos contables de ventas y cobranza armados con el motor de plantillas (reemplazan a los SPs del
    /// legacy co_asipro_ventas, co_asi_cancelacionr_x y a la función fc_genasicancelacion, que escribían las
    /// cuentas en el programa). Lo usan Nota de Débito, Cheques y Cancelaciones.
    ///
    /// No guarda: quien lo llama decide el SaveChanges (para hacerlo dentro de su transacción).
    /// </summary>
    public static class AsientoVentaService
    {
        public static async Task<PlantillaAsiento> GetPlantillaAsync(
            IUnitOfWork uow, ProcesoAsiento proceso, CancellationToken cancellationToken)
            => await uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                   .AsNoTracking()
                   .Include(p => p.Lines)
                   .FirstOrDefaultAsync(p => p.Process == proceso && p.IsActive, cancellationToken)
               ?? throw new InvalidOperationException(
                   $"No existe una plantilla de asiento activa para el proceso {proceso}.");

        /// <summary>
        /// Crea el asiento del documento (con bloqueo de correlativo por año, mes y tipo de la plantilla) con las
        /// líneas de todas las fuentes. El período contable de la fecha debe estar abierto
        /// (legacy: verificación de tcontroles.anomes_cierre).
        /// </summary>
        public static async Task<AsientoContable> BuildAsync(
            IUnitOfWork uow, PlantillaAsiento plantilla, IReadOnlyList<AsientoFuente> fuentes,
            DateTime fecha, string descripcion, decimal tipoCambio, bool aplicarDestinos,
            CancellationToken cancellationToken)
        {
            var anio = fecha.Year.ToString("0000");
            var mes = fecha.Month.ToString("00");

            await PeriodoContableRules.EnsureOpenAsync(uow, anio, mes, cancellationToken);

            await uow.AcquireAppLockAsync(
                AsientoNumeroGenerator.LockResource(anio, mes, plantilla.TipoAsientoCode), 15000, cancellationToken);

            var numero = await AsientoNumeroGenerator.NextAsync(uow, anio, mes, plantilla.TipoAsientoCode, cancellationToken);

            var asiento = new AsientoContable
            {
                Anio = anio,
                Mes = mes,
                TipoAsientoCode = plantilla.TipoAsientoCode,
                Numero = numero,
                Descripcion = AsientoNotaIngresoRules.Truncate(descripcion, 50),
                FechaProceso = fecha.Date,
                TipoCambio = tipoCambio,
                Estado = EstadoAsiento.Pendiente,
                Origen = OrigenAsiento.Ventas
            };

            foreach (var fuente in fuentes)
            {
                await AsientoPlantillaMotor.BuildAsync(uow, plantilla, fuente, asiento, cancellationToken);
            }

            if (aplicarDestinos)
            {
                await AsientoDestinoGenerator.ApplyAsync(uow, asiento, cancellationToken);
            }

            AsientoCuadre.EnsureBalanced(asiento);

            await uow.Contabilidad.Transacciones.AsientosContables.AddAsync(asiento, cancellationToken);

            return asiento;
        }

        /// <summary>
        /// Abre un asiento sin plantilla (reversas, como el rebote de cheque): valida el período, bloquea y toma el
        /// correlativo del tipo. Las líneas las agrega quien llama y luego <see cref="FinishAsync"/>.
        /// </summary>
        public static async Task<AsientoContable> NewAsync(
            IUnitOfWork uow, string tipoAsientoCode, DateTime fecha, string descripcion, decimal tipoCambio,
            CancellationToken cancellationToken)
        {
            var anio = fecha.Year.ToString("0000");
            var mes = fecha.Month.ToString("00");

            await PeriodoContableRules.EnsureOpenAsync(uow, anio, mes, cancellationToken);

            await uow.AcquireAppLockAsync(
                AsientoNumeroGenerator.LockResource(anio, mes, tipoAsientoCode), 15000, cancellationToken);

            var numero = await AsientoNumeroGenerator.NextAsync(uow, anio, mes, tipoAsientoCode, cancellationToken);

            return new AsientoContable
            {
                Anio = anio,
                Mes = mes,
                TipoAsientoCode = tipoAsientoCode,
                Numero = numero,
                Descripcion = AsientoNotaIngresoRules.Truncate(descripcion, 50),
                FechaProceso = fecha.Date,
                TipoCambio = tipoCambio,
                Estado = EstadoAsiento.Pendiente,
                Origen = OrigenAsiento.Ventas
            };
        }

        /// <summary>Cierra un asiento abierto con <see cref="NewAsync"/>: destinos, cuadre y alta.</summary>
        public static async Task FinishAsync(
            IUnitOfWork uow, AsientoContable asiento, bool aplicarDestinos, CancellationToken cancellationToken)
        {
            AsientoCuadre.Totalize(asiento);

            if (aplicarDestinos)
            {
                await AsientoDestinoGenerator.ApplyAsync(uow, asiento, cancellationToken);
            }

            AsientoCuadre.EnsureBalanced(asiento);

            await uow.Contabilidad.Transacciones.AsientosContables.AddAsync(asiento, cancellationToken);
        }

        /// <summary>Importe en soles de un monto en la moneda del documento (moneda base: tal cual).</summary>
        public static decimal ToSoles(decimal amount, string monedaCode, decimal exchangeRate)
            => monedaCode == DocumentoVentaRules.MonedaBase
                ? amount
                : DocumentoVentaRules.Round2(amount * exchangeRate);
    }
}
