using System.Data;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Logistica.Enum;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.Common
{
    /// <summary>
    /// Generación del asiento de consumo (legacy: evento ue_generaasiento de w_con_asientosconsumo), en UNA
    /// transacción y guiada por la plantilla activa del proceso ConsumoVales.
    ///
    /// Se genera UN asiento por planta y rango de fechas con todas las líneas de vale de salida Procesadas que aún no
    /// tienen asiento (fecha de entrega del vale dentro del rango; período = mes de la fecha final):
    ///   - Una línea al Debe por cada línea de vale (importe = total de la línea) en la planta afectada del vale, con
    ///     el número del vale como documento. La cuenta sale del subcentro de costo (y el anexo de ArticuloAnexo o
    ///     SubFamiliaAnexo); para los tipos de vale con línea propia en la plantilla (por ejemplo "TRA") se usa la
    ///     cuenta de esa línea.
    ///   - Una sola línea al Haber (concepto TotalVales) por el total, en la planta que entrega y con el número de
    ///     documento del último vale (igual que el legacy).
    ///   - Luego se generan los destinos y se exige Debe = Haber.
    /// Se marcan con el asiento las líneas de vale y sus vales.
    /// </summary>
    public static class AsientoConsumoGenerator
    {
        public static Task<AsientoConsumoResultResponse> GenerateAsync(
            IUnitOfWork uow, string plantaCode, DateTime fechaDesde, DateTime fechaHasta, CancellationToken cancellationToken)
        {
            var planta = plantaCode.Trim().ToUpperInvariant();

            return uow.ExecuteInTransactionAsync(
                token => GenerateCoreAsync(uow, planta, fechaDesde.Date, fechaHasta.Date, token),
                IsolationLevel.ReadCommitted,
                cancellationToken);
        }

        private static async Task<AsientoConsumoResultResponse> GenerateCoreAsync(
            IUnitOfWork uow, string planta, DateTime fechaDesde, DateTime fechaHasta, CancellationToken ct)
        {
            var hasta = fechaHasta.AddDays(1);

            var lineas = await uow.Logistica.Transacciones.ValeDetalles.Query()
                .Include(d => d.Vale).ThenInclude(v => v.SubCentroCosto)
                .Where(d => d.PlantaCode == planta
                            && d.Estado == EstadoValeDetalle.Procesado
                            && d.Vale.FechaEntrega >= fechaDesde
                            && d.Vale.FechaEntrega < hasta
                            && (d.AsientoContable == null || d.AsientoContable == ""))
                .OrderBy(d => d.PlantaCode).ThenBy(d => d.ValeCode).ThenBy(d => d.ItemNumber)
                .ToListAsync(ct);

            if (lineas.Count == 0)
            {
                throw new InvalidOperationException("No se registran vales de salida pendientes de asiento ... Verifique ...");
            }

            foreach (var linea in lineas)
            {
                if (Math.Round(linea.Total, 2, MidpointRounding.AwayFromZero) <= 0)
                {
                    throw new InvalidOperationException(
                        $"Vale de Salida nro: {linea.ValeCode} tiene total del artículo {linea.ArticuloCode} en cero ... Verifique ...");
                }
            }

            var plantilla = await uow.Contabilidad.Maestros.PlantillasAsiento.Query()
                .AsNoTracking()
                .Include(p => p.Lines)
                .FirstOrDefaultAsync(p => p.Process == ProcesoAsiento.ConsumoVales && p.IsActive, ct)
                ?? throw new InvalidOperationException(
                    $"No existe una plantilla de asiento activa para el proceso {ProcesoAsiento.ConsumoVales}.");

            var anio = fechaHasta.Year.ToString("0000");
            var mes = fechaHasta.Month.ToString("00");

            await PeriodoContableRules.EnsureOpenAsync(uow, anio, mes, ct);

            var lineasDetalle = plantilla.Lines.Where(l => l.Concept == ConceptoAsiento.DetalleVale).ToList();
            var lineaGeneral = lineasDetalle.FirstOrDefault(l => string.IsNullOrWhiteSpace(l.ValeTypeCode))
                ?? throw new InvalidOperationException("La plantilla de consumo no tiene la línea DetalleVale general.");
            var lineaTotal = plantilla.Lines.FirstOrDefault(l => l.Concept == ConceptoAsiento.TotalVales)
                ?? throw new InvalidOperationException("La plantilla de consumo no tiene la línea TotalVales.");

            var texto = (plantilla.DescriptionText ?? string.Empty).Trim();
            var comprobante = plantilla.ComprobantePagoCode;
            if (texto.Length == 0 || string.IsNullOrWhiteSpace(comprobante))
            {
                throw new InvalidOperationException(
                    "La plantilla de consumo debe definir el texto de la descripción y el comprobante de pago.");
            }

            // Se valida cuenta y anexo de cada artículo en su subcentro (como el legacy, para todos los tipos de vale).
            var resueltas = new Dictionary<(string Scc, string Art), AnexoEnlaceResolver.Resultado>();
            foreach (var linea in lineas)
            {
                var clave = (linea.Vale.SubCentroCostoCode.Trim(), linea.ArticuloCode.Trim());
                if (!resueltas.ContainsKey(clave))
                {
                    resueltas[clave] = await AnexoEnlaceResolver.ResolverAsync(
                        uow,
                        linea.Vale.SubCentroCostoCode,
                        linea.Vale.SubCentroCosto?.AccountingAccountCode,
                        linea.ArticuloCode,
                        ct);
                }
            }

            // Tipo de cambio: compra del día de proceso (legacy: gd_compra). Sin registro queda en cero.
            var hoy = DateOnly.FromDateTime(HoraLocal.Ahora());
            var compra = await uow.Comunes.TiposCambio.Query()
                .AsNoTracking()
                .Where(t => t.Date == hoy)
                .Select(t => (decimal?)t.BuyRate)
                .FirstOrDefaultAsync(ct) ?? 0m;

            await uow.AcquireAppLockAsync(
                AsientoNumeroGenerator.LockResource(anio, mes, plantilla.TipoAsientoCode), 15000, ct);

            var numero = await AsientoNumeroGenerator.NextAsync(uow, anio, mes, plantilla.TipoAsientoCode, ct);

            var asiento = new AsientoContable
            {
                Anio = anio,
                Mes = mes,
                TipoAsientoCode = plantilla.TipoAsientoCode,
                Numero = numero,
                Descripcion = AsientoNotaIngresoRules.Truncate(texto, 500),
                FechaProceso = fechaHasta,
                TipoCambio = compra,
                Estado = EstadoAsiento.Pendiente,
                Origen = OrigenAsiento.Logistica
            };

            var descripcionLinea = AsientoNotaIngresoRules.Truncate(texto, AsientoNotaIngresoRules.DescripcionLineaMax);
            var anexosVerificados = new HashSet<string>(StringComparer.Ordinal);
            var item = 0;
            var total = 0m;

            foreach (var linea in lineas)
            {
                var vale = linea.Vale;
                var tl = lineasDetalle.FirstOrDefault(l =>
                             string.Equals(l.ValeTypeCode?.Trim(), vale.TipoValeCode.Trim(), StringComparison.OrdinalIgnoreCase))
                         ?? lineaGeneral;

                var res = resueltas[(vale.SubCentroCostoCode.Trim(), linea.ArticuloCode.Trim())];

                var cuenta = tl.AccountSource == OrigenCuentaAsiento.Fija
                    ? (tl.AccountCode ?? string.Empty).Trim()
                    : res.CuentaCode;

                string? anexoTipo = null;
                string? anexoCode = null;
                if (tl.AnnexSource == OrigenAnexoAsiento.ArticuloSubCentro && res.AnexoCode is not null)
                {
                    anexoTipo = res.AnexoTipoCode;
                    anexoCode = res.AnexoCode;

                    var clave = anexoTipo + "|" + anexoCode;
                    if (anexosVerificados.Add(clave)
                        && !await uow.Contabilidad.Maestros.Anexos.Query().AsNoTracking()
                            .AnyAsync(a => a.TipoAnexoCode == anexoTipo && a.Code == anexoCode, ct))
                    {
                        throw new InvalidOperationException(
                            $"El anexo {anexoCode} no está registrado en el tipo de anexo {anexoTipo}.");
                    }
                }

                var importe = Math.Round(linea.Total, 2, MidpointRounding.AwayFromZero);
                total += importe;

                item++;
                asiento.Detalles.Add(new AsientoContableDetalle
                {
                    Anio = anio,
                    Mes = mes,
                    TipoAsientoCode = plantilla.TipoAsientoCode,
                    Numero = numero,
                    ItemNumber = item,
                    PlantaCode = vale.PlantaAfectadaCode,
                    EsDestino = tl.IsDestination,
                    Descripcion = descripcionLinea,
                    CuentaContableCode = cuenta,
                    AnexoTipoCode = anexoTipo,
                    AnexoCode = anexoCode,
                    NumeroDocumento = vale.Code,
                    CargoSol = tl.Nature == NaturalezaAsiento.Debe ? importe : 0m,
                    AbonoSol = tl.Nature == NaturalezaAsiento.Haber ? importe : 0m,
                    TipoContabilidad = tl.AccountingType,
                    PlantaEjecutoraCode = vale.PlantaAfectadaCode,
                    ComprobantePagoCode = comprobante
                });
            }

            // Línea única al Haber por el total: planta que entrega y documento del último vale (igual que el legacy).
            item++;
            asiento.Detalles.Add(new AsientoContableDetalle
            {
                Anio = anio,
                Mes = mes,
                TipoAsientoCode = plantilla.TipoAsientoCode,
                Numero = numero,
                ItemNumber = item,
                PlantaCode = planta,
                EsDestino = lineaTotal.IsDestination,
                Descripcion = descripcionLinea,
                CuentaContableCode = (lineaTotal.AccountCode ?? string.Empty).Trim(),
                NumeroDocumento = lineas[^1].ValeCode,
                CargoSol = lineaTotal.Nature == NaturalezaAsiento.Debe ? total : 0m,
                AbonoSol = lineaTotal.Nature == NaturalezaAsiento.Haber ? total : 0m,
                TipoContabilidad = lineaTotal.AccountingType,
                PlantaEjecutoraCode = planta,
                ComprobantePagoCode = comprobante
            });

            await AsientoDestinoGenerator.ApplyAsync(uow, asiento, ct);

            AsientoCuadre.EnsureBalanced(asiento);

            await uow.Contabilidad.Transacciones.AsientosContables.AddAsync(asiento, ct);

            // Marca las líneas de vale y los vales con el asiento.
            foreach (var linea in lineas)
            {
                linea.AsientoContable = asiento.Codigo;
            }

            foreach (var vale in lineas.Select(l => l.Vale).Distinct())
            {
                vale.AsientoContable = asiento.Codigo;
            }

            await uow.SaveChangesAsync(ct);

            return new AsientoConsumoResultResponse
            {
                PlantaCode = planta,
                Success = true,
                AsientoCodigo = asiento.Codigo,
                Lineas = lineas.Count,
                Vales = lineas.Select(l => l.ValeCode).Distinct().Count(),
                TotalSol = total,
                Message = $"Asiento {asiento.Codigo} generado."
            };
        }
    }
}
