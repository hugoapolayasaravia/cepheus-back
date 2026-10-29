using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common
{
    /// <summary>
    /// Regla de anulación de un asiento (legacy: fc_anulaasiento, camino de período abierto), sin guardar:
    /// quien la usa decide cuándo hacer SaveChanges (permite anular dentro de una transacción mayor).
    ///   - No se anula el tipo 04 (apertura).
    ///   - El período contable debe existir y estar Abierto.
    ///   - Se elimina el detalle, el asiento queda Anulado con totales en cero.
    ///   - Se libera la referencia al asiento en la Nota de Ingreso, la Orden de Servicio (con su salida y las
    ///     líneas de la salida), el Vale (con sus líneas) y el Documento de Compra que lo tuvieran.
    /// </summary>
    public static class AsientoAnulacionService
    {
        public static async Task<AsientoContable> AnularAsync(
            IUnitOfWork uow, string codigoAsiento, CancellationToken cancellationToken)
        {
            var (anio, mes, tipo, numero) = AsientoContableRules.ParseCodigo(codigoAsiento);
            var codigo = anio + mes + tipo + numero;

            if (tipo == AsientoContableRules.TipoAsientoApertura)
            {
                throw new InvalidOperationException("No puede anularse el tipo 04 = Asiento de Apertura.");
            }

            await PeriodoContableRules.EnsureOpenAsync(uow, anio, mes, cancellationToken);

            var asiento = await uow.Contabilidad.Transacciones.AsientosContables.Query()
                .Include(a => a.TipoAsiento)
                .Include(a => a.Detalles)
                .FirstOrDefaultAsync(
                    a => a.Anio == anio && a.Mes == mes && a.TipoAsientoCode == tipo && a.Numero == numero,
                    cancellationToken);

            if (asiento is null)
            {
                throw new KeyNotFoundException($"Asiento {codigo} no encontrado.");
            }

            if (asiento.Estado == EstadoAsiento.Anulado)
            {
                throw new InvalidOperationException($"El asiento {codigo} ya está anulado.");
            }

            foreach (var detalle in asiento.Detalles.ToList())
            {
                uow.Contabilidad.Transacciones.AsientoContableDetalles.Remove(detalle);
            }

            asiento.Detalles.Clear();
            asiento.Estado = EstadoAsiento.Anulado;
            asiento.TotalCargosSol = 0;
            asiento.TotalAbonosSol = 0;

            foreach (var n in await uow.Logistica.Transacciones.NotaIngresos.Query()
                         .Where(n => n.AsientoContable == codigo).ToListAsync(cancellationToken))
            {
                n.AsientoContable = null;
            }

            foreach (var o in await uow.Logistica.Transacciones.OrdenesServicio.Query()
                         .Where(o => o.AsientoContable == codigo).ToListAsync(cancellationToken))
            {
                o.AsientoContable = null;
            }

            foreach (var s in await uow.Logistica.Transacciones.OrdenesServicioSalida.Query()
                         .Where(s => s.AsientoContable == codigo).ToListAsync(cancellationToken))
            {
                s.AsientoContable = null;
            }

            foreach (var sd in await uow.Logistica.Transacciones.OrdenServicioSalidaDetalles.Query()
                         .Where(sd => sd.AsientoContable == codigo).ToListAsync(cancellationToken))
            {
                sd.AsientoContable = null;
            }

            foreach (var v in await uow.Logistica.Transacciones.Vales.Query()
                         .Where(v => v.AsientoContable == codigo).ToListAsync(cancellationToken))
            {
                v.AsientoContable = null;
            }

            foreach (var vd in await uow.Logistica.Transacciones.ValeDetalles.Query()
                         .Where(vd => vd.AsientoContable == codigo).ToListAsync(cancellationToken))
            {
                vd.AsientoContable = null;
            }

            foreach (var c in await uow.Contabilidad.Transacciones.CajasChica.Query()
                         .Where(c => c.ProvisionAsientoCode == codigo || c.ReversionAsientoCode == codigo)
                         .ToListAsync(cancellationToken))
            {
                if (c.ProvisionAsientoCode == codigo)
                {
                    c.ProvisionAsientoCode = null;
                }

                if (c.ReversionAsientoCode == codigo)
                {
                    c.ReversionAsientoCode = null;
                }
            }

            foreach (var d in await uow.Contabilidad.Transacciones.DocumentosCompra.Query()
                         .Where(d => d.AsientoProvisionCode == codigo).ToListAsync(cancellationToken))
            {
                d.AsientoProvisionCode = null;
            }

            return asiento;
        }
    }
}
