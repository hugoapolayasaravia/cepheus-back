using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common
{
    /// <summary>Importes de una línea de caja chica expresados en soles.</summary>
    public sealed class CajaChicaLineaSoles
    {
        public decimal Parcial { get; init; }
        public decimal Igv { get; init; }
        public decimal NoGravable { get; init; }
        public decimal Renta { get; init; }
        public decimal Fonavi { get; init; }
        public decimal Servicio { get; init; }
        public decimal IgvExterior { get; init; }
        public decimal Total { get; init; }
    }

    /// <summary>
    /// Arma los asientos de caja chica con el motor de plantillas (reemplaza a los SPs
    /// co_asipro_cajachica y co_asirev_cajachica con cuentas escritas en código).
    /// </summary>
    public static class CajaChicaAsientoService
    {
        /// <summary>
        /// Convierte los importes de la línea a soles con su tipo de cambio (como el SP co_exporta_cajachica_doc_compras):
        /// cada importe se redondea a 2 decimales y el total se recalcula con la fórmula del documento.
        /// En boletas (03) el no gravable es el parcial.
        /// </summary>
        public static CajaChicaLineaSoles ToSoles(CajaChicaDetalle d)
        {
            var esBase = TipoCambioRules.EsMonedaBase(d.MonedaCode);

            decimal S(decimal x) => esBase ? x : Math.Round(x * d.TipoCambio, 2, MidpointRounding.AwayFromZero);

            var parcial = S(d.Parcial);
            var igv = S(d.Igv);
            var renta = S(d.Renta);
            var fonavi = S(d.Fonavi);
            var servicio = S(d.Servicio);
            var igvExterior = S(d.IgvExterior);
            var noGravable = d.ComprobantePagoCode == "03" ? parcial : S(d.NoGravable);

            return new CajaChicaLineaSoles
            {
                Parcial = parcial,
                Igv = igv,
                NoGravable = noGravable,
                Renta = renta,
                Fonavi = fonavi,
                Servicio = servicio,
                IgvExterior = igvExterior,
                Total = parcial + igv - renta - fonavi + servicio + igvExterior
            };
        }

        public static async Task<PlantillaAsiento> GetPlantillaAsync(
            IUnitOfWork uow, ProcesoAsiento proceso, CancellationToken cancellationToken)
            => await uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .AsNoTracking()
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Process == proceso && p.IsActive, cancellationToken)
               ?? throw new InvalidOperationException(
                   $"No existe una plantilla de asiento activa para el proceso {proceso}.");

        public static async Task<Dictionary<string, Proveedor>> LoadProveedoresAsync(
            IUnitOfWork uow, IEnumerable<CajaChicaDetalle> lines, CancellationToken cancellationToken)
        {
            var codes = lines.Select(l => l.ProveedorCode).Distinct().ToList();

            var proveedores = await uow.Logistica.Maestros.Proveedores.Query()
                .AsNoTracking()
                .Where(p => codes.Contains(p.Code))
                .ToListAsync(cancellationToken);

            return proveedores.ToDictionary(p => p.Code);
        }

        /// <summary>Una fuente de asiento por cada línea (documento) de la caja.</summary>
        public static async Task<List<AsientoFuente>> BuildFuentesAsync(
            IUnitOfWork uow, CajaChica caja, PlantillaAsiento plantilla,
            IReadOnlyDictionary<string, Proveedor> proveedores, CancellationToken cancellationToken)
        {
            var requiereCuentaPorPagar = plantilla.Lines
                .Any(l => l.AccountSource == OrigenCuentaAsiento.CuentaPorPagarProveedor);

            var fuentes = new List<AsientoFuente>();

            foreach (var d in caja.Details.OrderBy(x => x.ItemNumber))
            {
                if (!proveedores.TryGetValue(d.ProveedorCode, out var proveedor))
                {
                    throw new InvalidOperationException($"Proveedor {d.ProveedorCode} no registrado.");
                }

                var s = ToSoles(d);

                var texto = string.IsNullOrWhiteSpace(d.Concepto)
                    ? $"{d.ComprobantePagoCode} {d.NumeroDocumento}".Trim()
                    : d.Concepto.Trim();
                texto = AsientoNotaIngresoRules.Truncate(texto, AsientoNotaIngresoRules.DescripcionLineaMax);

                fuentes.Add(new AsientoFuente
                {
                    PlantaCode = d.PlantaCode,
                    Glosa = texto,
                    NumeroDocumento = d.NumeroDocumento,
                    ComprobantePagoCode = d.ComprobantePagoCode,
                    EsNotaCredito = d.ComprobantePagoCode == AsientoNotaIngresoRules.NotaCreditoComprobanteCode,
                    ProveedorCode = d.ProveedorCode,
                    ProveedorNombre = proveedor.LegalName,
                    CuentaPorPagarCode = requiereCuentaPorPagar
                        ? await ProveedorCuentaPorPagarResolver.ResolverAsync(uow, d.ProveedorCode, d.MonedaCode, cancellationToken)
                        : null,
                    Total = s.Total,
                    Igv = s.Igv,
                    IgvExterior = s.IgvExterior,
                    Renta = s.Renta,
                    Fonavi = s.Fonavi,
                    NoGravable = s.NoGravable,
                    Servicio = s.Servicio,
                    Lineas = new List<AsientoFuenteLinea>
                    {
                        new()
                        {
                            PlantaCode = d.PlantaCode,
                            Detalle = texto,
                            CuentaCode = d.CuentaContableCode,
                            AnexoTipoCode = d.AnexoTipoCode,
                            AnexoCode = d.AnexoCode,
                            Parcial = s.Parcial
                        }
                    }
                });
            }

            return fuentes;
        }

        /// <summary>
        /// Crea el asiento (con bloqueo de correlativo por año, mes y tipo) y le agrega las líneas de todos
        /// los documentos con el motor de plantillas. Debe ejecutarse dentro de una transacción.
        /// Los destinos solo se generan para la provisión (la reversión no tiene cuentas de gasto).
        /// </summary>
        public static async Task<AsientoContable> BuildAsientoAsync(
            IUnitOfWork uow, PlantillaAsiento plantilla, IReadOnlyList<AsientoFuente> fuentes,
            string anio, string mes, DateTime fechaProceso, string descripcion, decimal tipoCambio,
            bool aplicarDestinos, CancellationToken cancellationToken)
        {
            await uow.AcquireAppLockAsync(
                AsientoNumeroGenerator.LockResource(anio, mes, plantilla.TipoAsientoCode), 15000, cancellationToken);

            var numero = await AsientoNumeroGenerator.NextAsync(uow, anio, mes, plantilla.TipoAsientoCode, cancellationToken);

            var asiento = new AsientoContable
            {
                Anio = anio,
                Mes = mes,
                TipoAsientoCode = plantilla.TipoAsientoCode,
                Numero = numero,
                Descripcion = descripcion,
                FechaProceso = fechaProceso.Date,
                TipoCambio = tipoCambio,
                Estado = EstadoAsiento.Pendiente,
                Origen = OrigenAsiento.CajaChica
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
    }
}
